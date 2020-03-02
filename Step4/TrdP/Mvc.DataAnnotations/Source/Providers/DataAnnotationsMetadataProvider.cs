using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
using TrdP.Localization.Abstractions;
using TrdP.Mvc.DataAnnotations.Localization.Attributes;

namespace TrdP.Mvc.DataAnnotations.Localization.Providers
{
    internal class DataAnnotationsMetadataProvider :
        IBindingMetadataProvider,
        IDisplayMetadataProvider,
        IValidationMetadataProvider
    {
        private readonly IStringLocalizerFactory _stringLocalizerFactory;

        public DataAnnotationsMetadataProvider(IStringLocalizerFactory stringLocalizerFactory)
        {
            _stringLocalizerFactory = stringLocalizerFactory ?? throw new ArgumentNullException(nameof(stringLocalizerFactory));
        }

        public void CreateBindingMetadata(BindingMetadataProviderContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            var editableAttribute = context.Attributes.OfType<EditableAttribute>().FirstOrDefault();
            if (editableAttribute != null)
            {
                context.BindingMetadata.IsReadOnly = !editableAttribute.AllowEdit;
            }
        }

        public void CreateDisplayMetadata(DisplayMetadataProviderContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            var attributes = context.Attributes;
            var dataTypeAttribute = attributes.OfType<DataTypeAttribute>().FirstOrDefault();
            var displayAttribute = attributes.OfType<DisplayAttribute>().FirstOrDefault();
            var displayColumnAttribute = attributes.OfType<DisplayColumnAttribute>().FirstOrDefault();
            var displayFormatAttribute = attributes.OfType<DisplayFormatAttribute>().FirstOrDefault();
            var displayNameAttribute = attributes.OfType<DisplayNameAttribute>().FirstOrDefault();
            var hiddenInputAttribute = attributes.OfType<HiddenInputAttribute>().FirstOrDefault();
            var scaffoldColumnAttribute = attributes.OfType<ScaffoldColumnAttribute>().FirstOrDefault();
            var uiHintAttribute = attributes.OfType<UIHintAttribute>().FirstOrDefault();

            // Special case the [DisplayFormat] attribute hanging off an applied [DataType] attribute. This property is
            // non-null for DataType.Currency, DataType.Date, DataType.Time, and potentially custom [DataType]
            // subclasses. The DataType.Currency, DataType.Date, and DataType.Time [DisplayFormat] attributes have a
            // non-null DataFormatString and the DataType.Date and DataType.Time [DisplayFormat] attributes have
            // ApplyFormatInEditMode==true.
            if (displayFormatAttribute == null && dataTypeAttribute != null)
            {
                displayFormatAttribute = dataTypeAttribute.DisplayFormat;
            }

            var displayMetadata = context.DisplayMetadata;

            if (displayFormatAttribute != null)
            {
                displayMetadata.ConvertEmptyStringToNull = displayFormatAttribute.ConvertEmptyStringToNull;
            }

            // DataTypeName
            if (dataTypeAttribute != null)
            {
                displayMetadata.DataTypeName = dataTypeAttribute.GetDataTypeName();
            }
            else if (displayFormatAttribute != null && !displayFormatAttribute.HtmlEncode)
            {
                displayMetadata.DataTypeName = DataType.Html.ToString();
            }

            var resourceSource = context.Key.ContainerType ?? context.Key.ModelType;
            var localizer = _stringLocalizerFactory.Create(resourceSource);

            // Description
            if (displayAttribute != null)
            {
                var hasLocalizableDescription = localizer != null && !string.IsNullOrEmpty(displayAttribute.Description) && displayAttribute.ResourceType == null;
                displayMetadata.Description = hasLocalizableDescription
                    ? (Func<string>) (() => localizer[displayAttribute.Description])
                    : () => displayAttribute.GetDescription();
            }

            // DisplayFormatString
            if (displayFormatAttribute != null)
            {
                displayMetadata.DisplayFormatString = displayFormatAttribute.DataFormatString;
            }

            // DisplayName
            // DisplayAttribute has precedence over DisplayNameAttribute.
            if (displayAttribute?.GetName() != null)
            {
                var hasLocalizableName = localizer != null && !string.IsNullOrEmpty(displayAttribute.Name) && displayAttribute.ResourceType == null;
                displayMetadata.DisplayName = hasLocalizableName
                    ? (Func<string>) (() => localizer[displayAttribute.Name])
                    : () => displayAttribute.GetName();
            }
            else if (displayNameAttribute != null)
            {
                var hasLocalizableDisplayName = localizer != null && !string.IsNullOrEmpty(displayNameAttribute.DisplayName);
                displayMetadata.DisplayName = hasLocalizableDisplayName
                    ? (Func<string>) (() => localizer[displayNameAttribute.DisplayName])
                    : () => displayNameAttribute.DisplayName;
            }

            // EditFormatString
            if (displayFormatAttribute != null && displayFormatAttribute.ApplyFormatInEditMode)
            {
                displayMetadata.EditFormatString = displayFormatAttribute.DataFormatString;
            }

            // IsEnum et cetera
            var underlyingType = Nullable.GetUnderlyingType(context.Key.ModelType) ?? context.Key.ModelType;
            var underlyingTypeInfo = underlyingType.GetTypeInfo();

            if (underlyingTypeInfo.IsEnum)
            {
                // IsEnum
                displayMetadata.IsEnum = true;

                // IsFlagsEnum
                displayMetadata.IsFlagsEnum = underlyingTypeInfo.IsDefined(typeof(FlagsAttribute), inherit: false);

                // EnumDisplayNamesAndValues and EnumNamesAndValues
                //
                // Order EnumDisplayNamesAndValues by DisplayAttribute.Order, then by the order of Enum.GetNames().
                // That method orders by absolute value, then its behavior is undefined (but hopefully stable).
                // Add to EnumNamesAndValues in same order but Dictionary does not guarantee order will be preserved.

                var groupedDisplayNamesAndValues = new List<KeyValuePair<EnumGroupAndName, string>>();
                var namesAndValues = new Dictionary<string, string>();

                var enumLocalizer = _stringLocalizerFactory.Create(underlyingType);

                var enumFields = Enum.GetNames(underlyingType)
                    .Select(name => underlyingType.GetField(name))
                    .OrderBy(field => field.GetCustomAttribute<DisplayAttribute>(inherit: false)?.GetOrder() ?? 1000);

                foreach (var field in enumFields)
                {
                    var groupName = GetDisplayGroup(field);
                    var value = ((Enum)field.GetValue(obj: null)).ToString("d");

                    groupedDisplayNamesAndValues.Add(new KeyValuePair<EnumGroupAndName, string>(
                        new EnumGroupAndName(
                            groupName,
                            () => GetDisplayName(field, enumLocalizer)),
                        value));
                    namesAndValues.Add(field.Name, value);
                }

                displayMetadata.EnumGroupedDisplayNamesAndValues = groupedDisplayNamesAndValues;
                displayMetadata.EnumNamesAndValues = namesAndValues;
            }

            // HasNonDefaultEditFormat
            if (!string.IsNullOrEmpty(displayFormatAttribute?.DataFormatString) &&
                displayFormatAttribute?.ApplyFormatInEditMode == true)
            {
                // Have a non-empty EditFormatString based on [DisplayFormat] from our cache.
                if (dataTypeAttribute == null)
                {
                    // Attributes include no [DataType]; [DisplayFormat] was applied directly.
                    displayMetadata.HasNonDefaultEditFormat = true;
                }
                else if (!dataTypeAttribute.DisplayFormat.Equals(displayFormatAttribute))
                {
                    // Attributes include separate [DataType] and [DisplayFormat]; [DisplayFormat] provided override.
                    displayMetadata.HasNonDefaultEditFormat = true;
                }
                else if (dataTypeAttribute.GetType() != typeof(DataTypeAttribute))
                {
                    // Attributes include [DisplayFormat] copied from [DataType] and [DataType] was of a subclass.
                    // Assume the [DataType] constructor used the protected DisplayFormat setter to override its
                    // default.  That is derived [DataType] provided override.
                    displayMetadata.HasNonDefaultEditFormat = true;
                }
            }

            // HideSurroundingHtml
            if (hiddenInputAttribute != null)
            {
                displayMetadata.HideSurroundingHtml = !hiddenInputAttribute.DisplayValue;
            }

            // HtmlEncode
            if (displayFormatAttribute != null)
            {
                displayMetadata.HtmlEncode = displayFormatAttribute.HtmlEncode;
            }

            // NullDisplayText
            if (displayFormatAttribute != null)
            {
                displayMetadata.NullDisplayText = displayFormatAttribute.NullDisplayText;
            }

            // Order
            if (displayAttribute?.GetOrder() != null)
            {
                displayMetadata.Order = displayAttribute.GetOrder().Value;
            }

            // Placeholder
            if (displayAttribute != null)
            {
                var hasLocalizablePrompt = localizer != null && !string.IsNullOrEmpty(displayAttribute.Prompt) && displayAttribute.ResourceType == null;
                displayMetadata.Placeholder = hasLocalizablePrompt
                    ? (Func<string>) (() => localizer[displayAttribute.Prompt])
                    : () => displayAttribute.GetPrompt();
            }

            // ShowForDisplay
            if (scaffoldColumnAttribute != null)
            {
                displayMetadata.ShowForDisplay = scaffoldColumnAttribute.Scaffold;
            }

            // ShowForEdit
            if (scaffoldColumnAttribute != null)
            {
                displayMetadata.ShowForEdit = scaffoldColumnAttribute.Scaffold;
            }

            // SimpleDisplayProperty
            if (displayColumnAttribute != null)
            {
                displayMetadata.SimpleDisplayProperty = displayColumnAttribute.DisplayColumn;
            }

            // TemplateHint
            if (uiHintAttribute != null)
            {
                displayMetadata.TemplateHint = uiHintAttribute.UIHint;
            }
            else if (hiddenInputAttribute != null)
            {
                displayMetadata.TemplateHint = "HiddenInput";
            }
        }

        public void CreateValidationMetadata(ValidationMetadataProviderContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            var attributes = new List<object>(context.Attributes.Count);

            for (var i = 0; i < context.Attributes.Count; i++)
            {
                var attribute = context.Attributes[i];
                if (attribute is ValidationProviderAttribute validationProviderAttribute)
                {
                    attributes.AddRange(validationProviderAttribute.GetValidationAttributes());
                }
                else
                {
                    attributes.Add(attribute);
                }
            }

            // RequiredAttribute marks a property as required by validation - this means that it
            // must have a non-null value on the model during validation.
            var requiredAttribute = attributes.OfType<RequiredAttribute>().FirstOrDefault();
            if (requiredAttribute != null)
            {
                context.ValidationMetadata.IsRequired = true;
            }

            foreach (var attribute in attributes.OfType<ValidationAttribute>())
            {
                // If another provider has already added this attribute, do not repeat it.
                // This will prevent attributes like RemoteAttribute (which implement ValidationAttribute and
                // IClientModelValidator) to be added to the ValidationMetadata twice.
                // This is to ensure we do not end up with duplication validation rules on the client side.
                if (!context.ValidationMetadata.ValidatorMetadata.Contains(attribute))
                {
                    context.ValidationMetadata.ValidatorMetadata.Add(attribute);
                }
            }
        }

        private static string GetDisplayName(FieldInfo field, IStringLocalizer stringLocalizer)
        {
            var display = field.GetCustomAttribute<DisplayAttribute>(inherit: false);
            if (display != null)
            {
                // Note [Display(Name = "")] is allowed but we will not attempt to localize the empty name.
                var name = display.GetName();
                if (stringLocalizer != null && !string.IsNullOrEmpty(name) && display.ResourceType == null)
                {
                    name = stringLocalizer[name];
                }

                return name ?? field.Name;
            }

            return field.Name;
        }

        private static string GetDisplayGroup(FieldInfo field)
        {
            var display = field.GetCustomAttribute<DisplayAttribute>(inherit: false);
            return display?.GetGroupName() ?? string.Empty;
        }
    }
}
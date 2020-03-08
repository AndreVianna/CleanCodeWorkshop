using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
using TrdP.Localization.Abstractions;

namespace TrdP.Mvc.DataAnnotations.Localization.ValidationProviders
{
    internal class MetadataDetailsProvider :
        ICompositeMetadataDetailsProvider
    {
        private readonly IStringLocalizerFactory _stringLocalizerFactory;

        public MetadataDetailsProvider(IStringLocalizerFactory stringLocalizerFactory)
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
            if (editableAttribute == null)
            {
                return;
            }

            context.BindingMetadata.IsReadOnly = !editableAttribute.AllowEdit;
        }

        public void CreateValidationMetadata(ValidationMetadataProviderContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (context.Attributes.OfType<RequiredAttribute>().Any())
            {
                context.ValidationMetadata.IsRequired = true;
            }

            foreach (var attribute in context.Attributes)
            {
                switch (attribute)
                {
                    case ValidationProviderAttribute validationProviderAttribute:
                        foreach (var validationAttribute in validationProviderAttribute.GetValidationAttributes())
                        {
                            AddValidationAttribute(context, validationAttribute);
                        }
                        break;
                    case ValidationAttribute validationAttribute:
                        AddValidationAttribute(context, validationAttribute);
                        break;
                }
            }
        }

        public void CreateDisplayMetadata(DisplayMetadataProviderContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }


            var resourceSource = context.Key.ContainerType ?? context.Key.ModelType;
            var stringLocalizer = _stringLocalizerFactory.Create(resourceSource);

            var scaffoldColumnAttribute = context.Attributes.OfType<ScaffoldColumnAttribute>().FirstOrDefault();
            SetDisplayMetadataScaffolding(context.DisplayMetadata, scaffoldColumnAttribute);

            var hiddenInputAttribute = context.Attributes.OfType<HiddenInputAttribute>().FirstOrDefault();
            SetDisplayMetadataHideSurroundingHtml(context.DisplayMetadata, hiddenInputAttribute);

            var uiHintAttribute = context.Attributes.OfType<UIHintAttribute>().FirstOrDefault();
            SetDisplayMetadataTemplateHint(context.DisplayMetadata, uiHintAttribute, hiddenInputAttribute);

            var displayAttribute = context.Attributes.OfType<DisplayAttribute>().FirstOrDefault();
            SetDisplayMetadataDescription(context.DisplayMetadata, displayAttribute, stringLocalizer);
            SetDisplayMetadataOrder(context.DisplayMetadata, displayAttribute);
            SetDisplayMetadataPlaceholder(context.DisplayMetadata, displayAttribute, stringLocalizer);

            var displayNameAttribute = context.Attributes.OfType<DisplayNameAttribute>().FirstOrDefault();
            SetDisplayMetadataDisplayName(context.DisplayMetadata, displayAttribute, displayNameAttribute, context.Key.Name, stringLocalizer);

            var displayColumnAttribute = context.Attributes.OfType<DisplayColumnAttribute>().FirstOrDefault();
            SetDisplayMetadataSimpleDisplayProperty(context.DisplayMetadata, displayColumnAttribute);

            var dataTypeAttribute = context.Attributes.OfType<DataTypeAttribute>().FirstOrDefault();
            var displayFormatAttribute = context.Attributes.OfType<DisplayFormatAttribute>().FirstOrDefault();
            if (displayFormatAttribute == null && dataTypeAttribute != null)
            {
                // Special case the [DisplayFormat] attribute hanging off an applied [DataType] attribute. This property is
                // non-null for DataType.Currency, DataType.Date, DataType.Time, and potentially custom [DataType]
                // subclasses. The DataType.Currency, DataType.Date, and DataType.Time [DisplayFormat] attributes have a
                // non-null DataFormatString and the DataType.Date and DataType.Time [DisplayFormat] attributes have
                // ApplyFormatInEditMode==true.
                displayFormatAttribute = dataTypeAttribute.DisplayFormat;
            }
            SetDisplayMetadataDataTypeName(context.DisplayMetadata, dataTypeAttribute, displayFormatAttribute);
            SetDisplayMetadataHasNonDefaultEditFormat(context.DisplayMetadata, dataTypeAttribute, displayFormatAttribute);
            SetDisplayMetadataConvertEmptyStringToNull(context.DisplayMetadata, displayFormatAttribute);
            SetDisplayMetadataDisplayFormatString(context.DisplayMetadata, displayFormatAttribute);
            SetDisplayMetadataEditFormatString(context.DisplayMetadata, displayFormatAttribute);
            SetDisplayMetadataHtmlEncode(context.DisplayMetadata, displayFormatAttribute);
            SetDisplayMetadataNullDisplayText(context.DisplayMetadata, displayFormatAttribute);

            SetDisplayMetadataEnumProperties(context.DisplayMetadata, context);
        }

        private static void AddValidationAttribute(ValidationMetadataProviderContext context, ValidationAttribute validationAttribute)
        {
            if (!context.ValidationMetadata.ValidatorMetadata.Contains(validationAttribute))
            {
                context.ValidationMetadata.ValidatorMetadata.Add(validationAttribute);
            }
        }

        private static void SetDisplayMetadataConvertEmptyStringToNull(DisplayMetadata displayMetadata, DisplayFormatAttribute displayFormatAttribute)
        {
            if (displayFormatAttribute == null)
            {
                return;
            }

            displayMetadata.ConvertEmptyStringToNull = displayFormatAttribute.ConvertEmptyStringToNull;
        }

        private static void SetDisplayMetadataDataTypeName(DisplayMetadata displayMetadata, DataTypeAttribute dataTypeAttribute, DisplayFormatAttribute displayFormatAttribute)
        {
            if (dataTypeAttribute == null && displayFormatAttribute == null)
            {
                return;
            }
            
            if (dataTypeAttribute != null)
            {
                displayMetadata.DataTypeName = dataTypeAttribute.GetDataTypeName();
                return;
            }
            
            if (!displayFormatAttribute.HtmlEncode)
            {
                displayMetadata.DataTypeName = DataType.Html.ToString();
            }
        }

        private static void SetDisplayMetadataDescription(DisplayMetadata displayMetadata, DisplayAttribute displayAttribute, IStringLocalizer stringLocalizer)
        {
            if (displayAttribute == null)
            {
                return;
            }

            displayMetadata.Description = () => stringLocalizer[displayAttribute.Description];
        }

        private static void SetDisplayMetadataDisplayFormatString(DisplayMetadata displayMetadata, DisplayFormatAttribute displayFormatAttribute)
        {
            if (displayFormatAttribute == null)
            {
                return;
            }

            displayMetadata.DisplayFormatString = displayFormatAttribute.DataFormatString;
        }

        private static void SetDisplayMetadataDisplayName(DisplayMetadata displayMetadata, DisplayAttribute displayAttribute, DisplayNameAttribute displayNameAttribute, string fieldName, IStringLocalizer stringLocalizer)
        {
            if (displayAttribute?.Name != null)
            {
                displayMetadata.DisplayName = () => stringLocalizer[displayAttribute.Name];
                return;
            }

            if (displayNameAttribute?.DisplayName != null)
            {
                displayMetadata.DisplayName = () => stringLocalizer[displayNameAttribute.DisplayName];
                return;
            }

            displayMetadata.DisplayName = () => stringLocalizer[fieldName];
        }

        private static void SetDisplayMetadataEditFormatString(DisplayMetadata displayMetadata, DisplayFormatAttribute displayFormatAttribute)
        {
            if (displayFormatAttribute == null || !displayFormatAttribute.ApplyFormatInEditMode)
            {
                return;
            }

            displayMetadata.EditFormatString = displayFormatAttribute.DataFormatString;
        }

        private void SetDisplayMetadataEnumProperties(DisplayMetadata displayMetadata, DisplayMetadataProviderContext context)
        {
            var underlyingType = Nullable.GetUnderlyingType(context.Key.ModelType) ?? context.Key.ModelType;
            var underlyingTypeInfo = underlyingType.GetTypeInfo();
            if (!underlyingTypeInfo.IsEnum)
            {
                return;
            }

            displayMetadata.IsEnum = true;
            displayMetadata.IsFlagsEnum = underlyingTypeInfo.IsDefined(typeof(FlagsAttribute), inherit: false);

            SetDisplayMetadataEnumNamesAndValues(displayMetadata, underlyingType);
        }

        private void SetDisplayMetadataEnumNamesAndValues(DisplayMetadata displayMetadata, Type underlyingType)
        {
            // Order EnumDisplayNamesAndValues by DisplayAttribute.Order, then by the order of Enum.GetNames().
            // That method orders by absolute value, then its behavior is undefined (but hopefully stable).
            // Add to EnumNamesAndValues in same order but Dictionary does not guarantee order will be preserved.
            var namesAndValues = new Dictionary<string, string>();
            var groupedDisplayNamesAndValues = new List<KeyValuePair<EnumGroupAndName, string>>();

            var stringLocalizer = _stringLocalizerFactory.Create(underlyingType);
            var enumFields = Enum.GetNames(underlyingType)
                .Select(underlyingType.GetField)
                .OrderBy(field => field.GetCustomAttribute<DisplayAttribute>(inherit: false)?.GetOrder() ?? 1000);
            foreach (var field in enumFields)
            {
                var value = ((Enum)field.GetValue(obj: null)).ToString("d");
                namesAndValues.Add(field.Name, value);

                var groupName = GetDisplayGroup(field);
                var enumGroupName = new EnumGroupAndName(groupName, () => GetEnumDisplayName(field, stringLocalizer));
                var item = new KeyValuePair<EnumGroupAndName, string>(enumGroupName, value);
                groupedDisplayNamesAndValues.Add(item);
            }
            displayMetadata.EnumNamesAndValues = namesAndValues;
            displayMetadata.EnumGroupedDisplayNamesAndValues = groupedDisplayNamesAndValues;
        }

        private static void SetDisplayMetadataHasNonDefaultEditFormat(DisplayMetadata displayMetadata, DataTypeAttribute dataTypeAttribute, DisplayFormatAttribute displayFormatAttribute)
        {
            if (string.IsNullOrEmpty(displayFormatAttribute?.DataFormatString) || displayFormatAttribute.ApplyFormatInEditMode != true)
            {
                return;
            }

            if (dataTypeAttribute == null)
            {
                // Attributes include no [DataType]; [DisplayFormat] was applied directly.
                displayMetadata.HasNonDefaultEditFormat = true;
                return;
            }

            if (!Equals(dataTypeAttribute.DisplayFormat, displayFormatAttribute))
            {
                // Attributes include separate [DataType] and [DisplayFormat]; [DisplayFormat] provided override.
                displayMetadata.HasNonDefaultEditFormat = true;
                return;
            }
            
            if (dataTypeAttribute.GetType() != typeof(DataTypeAttribute))
            {
                // Attributes include [DisplayFormat] copied from [DataType] and [DataType] was of a subclass.
                // Assume the [DataType] constructor used the protected DisplayFormat setter to override its
                // default.  That is derived [DataType] provided override.
                displayMetadata.HasNonDefaultEditFormat = true;
            }
        }

        private static void SetDisplayMetadataHideSurroundingHtml(DisplayMetadata displayMetadata, HiddenInputAttribute hiddenInputAttribute)
        {
            if (hiddenInputAttribute == null)
            {
                return;
            }

            displayMetadata.HideSurroundingHtml = !hiddenInputAttribute.DisplayValue;
        }

        private static void SetDisplayMetadataHtmlEncode(DisplayMetadata displayMetadata, DisplayFormatAttribute displayFormatAttribute)
        {
            if (displayFormatAttribute == null)
            {
                return;
            }

            displayMetadata.HtmlEncode = displayFormatAttribute.HtmlEncode;
        }

        private static void SetDisplayMetadataNullDisplayText(DisplayMetadata displayMetadata, DisplayFormatAttribute displayFormatAttribute)
        {
            if (displayFormatAttribute == null)
            {
                return;
            }

            displayMetadata.NullDisplayText = displayFormatAttribute.NullDisplayText;
        }

        private static void SetDisplayMetadataOrder(DisplayMetadata displayMetadata, DisplayAttribute displayAttribute)
        {
            var order = displayAttribute?.GetOrder();
            if (order == null)
            {
                return;
            }

            displayMetadata.Order = order.Value;
        }

        private static void SetDisplayMetadataPlaceholder(DisplayMetadata displayMetadata, DisplayAttribute displayAttribute, IStringLocalizer stringLocalizer)
        {
            if (displayAttribute == null)
            {
                return;
            }

            displayMetadata.Placeholder = () => stringLocalizer[displayAttribute.Prompt];
        }

        private static void SetDisplayMetadataScaffolding(DisplayMetadata displayMetadata, ScaffoldColumnAttribute scaffoldColumnAttribute)
        {
            if (scaffoldColumnAttribute == null)
            {
                return;
            }

            displayMetadata.ShowForDisplay = scaffoldColumnAttribute.Scaffold;
            displayMetadata.ShowForEdit = scaffoldColumnAttribute.Scaffold;
        }

        private static void SetDisplayMetadataSimpleDisplayProperty(DisplayMetadata displayMetadata, DisplayColumnAttribute displayColumnAttribute)
        {
            if (displayColumnAttribute == null)
            {
                return;
            }

            displayMetadata.SimpleDisplayProperty = displayColumnAttribute.DisplayColumn;
        }

        private static void SetDisplayMetadataTemplateHint(DisplayMetadata displayMetadata, UIHintAttribute uiHintAttribute, HiddenInputAttribute hiddenInputAttribute)
        {
            if (uiHintAttribute == null && hiddenInputAttribute == null)
            {
                return;
            }
            
            if (uiHintAttribute != null)
            {
                displayMetadata.TemplateHint = uiHintAttribute.UIHint;
                return;
            }
            
            displayMetadata.TemplateHint = "HiddenInput";
        }

#pragma warning disable IDE0046 // Convert to conditional expression
        private static string GetEnumDisplayName(FieldInfo field, IStringLocalizer stringLocalizer)
        {
            var displayAttribute = field.GetCustomAttribute<DisplayAttribute>(inherit: false);
            if (displayAttribute?.Name == null)
            {
                return stringLocalizer[field.Name];
            }

            return stringLocalizer[displayAttribute.Name];
        }
#pragma warning restore IDE0046 // Convert to conditional expression

        private static string GetDisplayGroup(FieldInfo field)
        {
            var display = field.GetCustomAttribute<DisplayAttribute>(inherit: false);
            return display?.GetGroupName() ?? string.Empty;
        }
    }
}
using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using TrdP.Localization.Abstractions;

namespace TrdP.Mvc.DataAnnotations.Localization.AttributeAdapters
{
    internal class CompareAttributeAdapter : ValidationAttributeAdapter<CompareAttribute>
    {
        private readonly string _otherProperty;

        public CompareAttributeAdapter(CompareAttribute attribute, IStringLocalizer stringLocalizer)
            : base(attribute, stringLocalizer)
        {
            _otherProperty = "*." + attribute.OtherProperty;
        }

        public override void AddValidation(ClientModelValidationContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            MergeAttribute(context.Attributes, "data-val", "true");
            MergeAttribute(context.Attributes, "data-val-equalto", GetErrorMessage(context));
            MergeAttribute(context.Attributes, "data-val-equalto-other", _otherProperty);
        }

        public override string GetErrorMessage(ModelValidationContextBase validationContext)
        {
            if (validationContext == null)
            {
                throw new ArgumentNullException(nameof(validationContext));
            }

            var displayName = validationContext.ModelMetadata.GetDisplayName();
            var otherPropertyDisplayName = GetOtherPropertyDisplayName(validationContext, Attribute);

            return GetLocalizedErrorMessage(displayName, otherPropertyDisplayName);
        }

        private string GetOtherPropertyDisplayName(ModelValidationContextBase validationContext, CompareAttribute attribute)
        {
            // The System.ComponentModel.DataAnnotations.CompareAttribute doesn't populate the
            // OtherPropertyDisplayName until after IsValid() is called. Therefore, at the time we get
            // the error message for client validation, the display name is not populated and won't be used.
            var otherPropertyDisplayName = attribute.OtherPropertyDisplayName;
            if (otherPropertyDisplayName != null || validationContext.ModelMetadata.ContainerType == null)
            {
                return attribute.OtherProperty;
            }

            var otherProperty = validationContext.MetadataProvider.GetMetadataForProperty(
                validationContext.ModelMetadata.ContainerType,
                attribute.OtherProperty);
            return otherProperty != null
                ? otherProperty.GetDisplayName()
                : attribute.OtherProperty;
        }
    }
}
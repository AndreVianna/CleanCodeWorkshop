using System;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using TrdP.Localization.Abstractions;

namespace TrdP.Mvc.DataAnnotations.Localization.AttributeAdapters
{
    internal class MinLengthAttributeAdapter : ValidationAttributeAdapter<MinLengthAttribute>
    {
        private readonly string _minLengthValue;

        public MinLengthAttributeAdapter(MinLengthAttribute attribute, IStringLocalizer stringLocalizer)
            : base(attribute, stringLocalizer)
        {
            _minLengthValue = Attribute.Length.ToString(CultureInfo.InvariantCulture);
        }

        public override void AddValidation(ClientModelValidationContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            MergeAttribute(context.Attributes, "data-val", "true");
            MergeAttribute(context.Attributes, "data-val-minlength", GetErrorMessage(context));
            MergeAttribute(context.Attributes, "data-val-minlength-min", _minLengthValue);
        }

        public override string GetErrorMessage(ModelValidationContextBase validationContext)
        {
            if (validationContext == null)
            {
                throw new ArgumentNullException(nameof(validationContext));
            }

            var displayName = validationContext.ModelMetadata.GetDisplayName();
            return GetLocalizedErrorMessage(displayName, Attribute.Length);
        }
    }
}
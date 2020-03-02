using System;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using TrdP.Localization.Abstractions;

namespace TrdP.Mvc.DataAnnotations.Localization.ClientModelValidators
{
    internal class StringLengthAttributeAdapter : ValidationAttributeAdapter<StringLengthAttribute>
    {
        private readonly string _maxLength;
        private readonly string _minLength;

        public StringLengthAttributeAdapter(StringLengthAttribute attribute, IStringLocalizer stringLocalizer)
            : base(attribute, stringLocalizer)
        {
            _maxLength = Attribute.MaximumLength.ToString(CultureInfo.InvariantCulture);
            _minLength = Attribute.MinimumLength.ToString(CultureInfo.InvariantCulture);
        }

        /// <inheritdoc />
        public override void AddValidation(ClientModelValidationContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            MergeAttribute(context.Attributes, "data-val", "true");
            MergeAttribute(context.Attributes, "data-val-length", GetErrorMessage(context));

            if (Attribute.MaximumLength != int.MaxValue)
            {
                MergeAttribute(context.Attributes, "data-val-length-max", _maxLength);
            }

            if (Attribute.MinimumLength != 0)
            {
                MergeAttribute(context.Attributes, "data-val-length-min", _minLength);
            }
        }

        /// <inheritdoc />
        public override string GetErrorMessage(ModelValidationContextBase validationContext)
        {
            if (validationContext == null)
            {
                throw new ArgumentNullException(nameof(validationContext));
            }

            return GetErrorMessage(
                validationContext.ModelMetadata,
                validationContext.ModelMetadata.GetDisplayName(),
                Attribute.MaximumLength,
                Attribute.MinimumLength);
        }
    }
}
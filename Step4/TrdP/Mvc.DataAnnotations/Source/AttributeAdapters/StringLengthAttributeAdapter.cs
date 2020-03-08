using System.ComponentModel.DataAnnotations;
using System.Globalization;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using TrdP.Localization.Abstractions;

namespace TrdP.Mvc.DataAnnotations.Localization.AttributeAdapters
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

        protected override void AddAdapterValidation(ClientModelValidationContext context)
        {
            MergeAttribute(context.Attributes, "data-val-length", GetErrorMessage(context));
            MergeAttribute(context.Attributes, "data-val-length-max", _maxLength);

            if (Attribute.MinimumLength != 0)
            {
                MergeAttribute(context.Attributes, "data-val-length-min", _minLength);
            }
        }

        protected override string GetAdapterErrorMessage(ModelValidationContextBase context)
        {
            var displayName = context.ModelMetadata.GetDisplayName();
            return Attribute.MinimumLength != 0
                ? StringLocalizer[ERROR_MESSAGE_WITH_MINIMUM, displayName, Attribute.MaximumLength, Attribute.MinimumLength]
                : GetLocalizedErrorMessage(displayName, Attribute.MaximumLength);
        }

        private const string ERROR_MESSAGE_WITH_MINIMUM =
            "The field {0} must be a string with a minimum length of {2} and a maximum length of {1}.";
    }
}
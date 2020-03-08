using System.ComponentModel.DataAnnotations;
using System.Globalization;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using TrdP.Localization.Abstractions;

namespace TrdP.Mvc.DataAnnotations.Localization.AttributeAdapters
{
    internal class MaxLengthAttributeAdapter : ValidationAttributeAdapter<MaxLengthAttribute>
    {
        private readonly string _maxLengthValue;

        public MaxLengthAttributeAdapter(MaxLengthAttribute attribute, IStringLocalizer stringLocalizer)
            : base(attribute, stringLocalizer)
        {
            _maxLengthValue = Attribute.Length.ToString(CultureInfo.InvariantCulture);
        }

        protected override void AddAdapterValidation(ClientModelValidationContext context)
        {
            MergeAttribute(context.Attributes, "data-val-maxlength", GetErrorMessage(context));
            MergeAttribute(context.Attributes, "data-val-maxlength-max", _maxLengthValue);
        }

        protected override string GetAdapterErrorMessage(ModelValidationContextBase context)
        {
            var displayName = context.ModelMetadata.GetDisplayName();
            return GetLocalizedErrorMessage(displayName, Attribute.Length);
        }
    }
}
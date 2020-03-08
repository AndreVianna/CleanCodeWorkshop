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

        protected override void AddAdapterValidation(ClientModelValidationContext context)
        {
            MergeAttribute(context.Attributes, "data-val-minlength", GetErrorMessage(context));
            MergeAttribute(context.Attributes, "data-val-minlength-min", _minLengthValue);
        }

        protected override string GetAdapterErrorMessage(ModelValidationContextBase context)
        {
            var displayName = context.ModelMetadata.GetDisplayName();
            return GetLocalizedErrorMessage(displayName, Attribute.Length);
        }
    }
}
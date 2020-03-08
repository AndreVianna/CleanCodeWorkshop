using System;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using TrdP.Localization.Abstractions;

namespace TrdP.Mvc.DataAnnotations.Localization.AttributeAdapters
{
    internal class RangeAttributeAdapter : ValidationAttributeAdapter<RangeAttribute>
    {
        private readonly string _maxValue;
        private readonly string _minValue;

        public RangeAttributeAdapter(RangeAttribute attribute, IStringLocalizer stringLocalizer)
            : base(attribute, stringLocalizer)
        {
            // This will trigger the conversion of Attribute.Minimum and Attribute.Maximum.
            // This is needed, because the attribute is stateful and will convert from a string like
            // "100m" to the decimal value 100.
            //
            // Validate a randomly selected number.
            attribute.IsValid(3);

            _maxValue = Convert.ToString(Attribute.Maximum, CultureInfo.InvariantCulture);
            _minValue = Convert.ToString(Attribute.Minimum, CultureInfo.InvariantCulture);
        }

        protected override void AddAdapterValidation(ClientModelValidationContext context)
        {
            MergeAttribute(context.Attributes, "data-val-range", GetErrorMessage(context));
            MergeAttribute(context.Attributes, "data-val-range-max", _maxValue);
            MergeAttribute(context.Attributes, "data-val-range-min", _minValue);
        }

        protected override string GetAdapterErrorMessage(ModelValidationContextBase context)
        {
            var displayName = context.ModelMetadata.GetDisplayName();
            return GetLocalizedErrorMessage(displayName, Attribute.Minimum, Attribute.Maximum);
        }
    }
}
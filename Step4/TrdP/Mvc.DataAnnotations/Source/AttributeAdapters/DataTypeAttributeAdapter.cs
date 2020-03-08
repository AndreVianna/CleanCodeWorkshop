using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using TrdP.Localization.Abstractions;

namespace TrdP.Mvc.DataAnnotations.Localization.AttributeAdapters
{
    internal class DataTypeAttributeAdapter : ValidationAttributeAdapter<DataTypeAttribute>
    {
        private readonly string _ruleName;

        public DataTypeAttributeAdapter(DataTypeAttribute attribute, string ruleName, IStringLocalizer stringLocalizer)
            : base(attribute, stringLocalizer)
        {
            if (string.IsNullOrWhiteSpace(ruleName))
            {
                throw new ArgumentException("Argument cannot be null or empty.", nameof(ruleName));
            }

            _ruleName = ruleName;
        }

        protected override void AddAdapterValidation(ClientModelValidationContext context)
        {
            MergeAttribute(context.Attributes, $"data-val-{_ruleName}", GetErrorMessage(context));
        }

        protected override string GetAdapterErrorMessage(ModelValidationContextBase context)
        {
            var displayName = context.ModelMetadata.GetDisplayName();
            return GetLocalizedErrorMessage(displayName, Attribute.GetDataTypeName());
        }
    }
}
using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using TrdP.Localization.Abstractions;

namespace TrdP.Mvc.DataAnnotations.Localization.AttributeAdapters
{
    internal class DataTypeAttributeAdapter : ValidationAttributeAdapter<DataTypeAttribute>
    {
        public DataTypeAttributeAdapter(DataTypeAttribute attribute, string ruleName, IStringLocalizer stringLocalizer)
            : base(attribute, stringLocalizer)
        {
            if (string.IsNullOrEmpty(ruleName))
            {
                throw new ArgumentException("Argument cannot be null or empty", nameof(ruleName));
            }

            RuleName = ruleName;
        }

        public string RuleName { get; }

        public override void AddValidation(ClientModelValidationContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            MergeAttribute(context.Attributes, "data-val", "true");
            MergeAttribute(context.Attributes, RuleName, GetErrorMessage(context));
        }

        public override string GetErrorMessage(ModelValidationContextBase validationContext)
        {
            if (validationContext == null)
            {
                throw new ArgumentNullException(nameof(validationContext));
            }

            var displayName = validationContext.ModelMetadata.GetDisplayName();
            return GetLocalizedErrorMessage(displayName, Attribute.GetDataTypeName());
        }
    }
}
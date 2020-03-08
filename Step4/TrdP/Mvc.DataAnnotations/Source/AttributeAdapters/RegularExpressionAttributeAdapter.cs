using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using TrdP.Localization.Abstractions;

namespace TrdP.Mvc.DataAnnotations.Localization.AttributeAdapters
{
    internal class RegularExpressionAttributeAdapter : ValidationAttributeAdapter<RegularExpressionAttribute>
    {
        public RegularExpressionAttributeAdapter(RegularExpressionAttribute attribute, IStringLocalizer stringLocalizer)
            : base(attribute, stringLocalizer)
        {
        }

        protected override void AddAdapterValidation(ClientModelValidationContext context)
        {
            MergeAttribute(context.Attributes, "data-val-regex", GetErrorMessage(context));
            MergeAttribute(context.Attributes, "data-val-regex-pattern", Attribute.Pattern);
        }

        protected override string GetAdapterErrorMessage(ModelValidationContextBase context)
        {
            var displayName = context.ModelMetadata.GetDisplayName();
            return GetLocalizedErrorMessage(displayName, Attribute.Pattern);
        }
    }
}
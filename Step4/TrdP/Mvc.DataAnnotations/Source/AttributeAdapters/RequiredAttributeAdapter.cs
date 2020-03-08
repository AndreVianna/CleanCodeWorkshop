using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using TrdP.Localization.Abstractions;

namespace TrdP.Mvc.DataAnnotations.Localization.AttributeAdapters
{
    internal class RequiredAttributeAdapter : ValidationAttributeAdapter<RequiredAttribute>
    {
        public RequiredAttributeAdapter(RequiredAttribute attribute, IStringLocalizer stringLocalizer)
            : base(attribute, stringLocalizer)
        {
        }

        protected override void AddAdapterValidation(ClientModelValidationContext context)
        {
            MergeAttribute(context.Attributes, "data-val-required", GetErrorMessage(context));
        }

        protected override string GetAdapterErrorMessage(ModelValidationContextBase context)
        {
            var displayName = context.ModelMetadata.GetDisplayName();
            return GetLocalizedErrorMessage(displayName);
        }
    }
}
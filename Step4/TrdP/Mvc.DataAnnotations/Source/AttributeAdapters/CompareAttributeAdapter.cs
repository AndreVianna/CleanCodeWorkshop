using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using TrdP.Localization.Abstractions;

namespace TrdP.Mvc.DataAnnotations.Localization.AttributeAdapters
{
    internal class CompareAttributeAdapter : ValidationAttributeAdapter<CompareAttribute>
    {
        private readonly string _otherProperty;

        public CompareAttributeAdapter(CompareAttribute attribute, IStringLocalizer stringLocalizer)
            : base(attribute, stringLocalizer)
        {
            _otherProperty = "*." + attribute.OtherProperty;
        }

        protected override void AddAdapterValidation(ClientModelValidationContext context)
        {
            MergeAttribute(context.Attributes, "data-val-equalto", GetErrorMessage(context));
            MergeAttribute(context.Attributes, "data-val-equalto-other", _otherProperty);
        }

        protected override string GetAdapterErrorMessage(ModelValidationContextBase context)
        {
            var displayName = context.ModelMetadata.GetDisplayName();
            var otherPropertyDisplayName = GetOtherPropertyDisplayName(context, Attribute);
            return GetLocalizedErrorMessage(displayName, otherPropertyDisplayName);
        }

        private string GetOtherPropertyDisplayName(ModelValidationContextBase context, CompareAttribute attribute)
        {
            var otherPropertyMetadata = context.MetadataProvider.GetMetadataForProperty(context.ModelMetadata.ContainerType, attribute.OtherProperty);
            return otherPropertyMetadata.GetDisplayName();
        }
    }
}
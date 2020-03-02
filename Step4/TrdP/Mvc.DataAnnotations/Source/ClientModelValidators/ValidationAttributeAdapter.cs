using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using TrdP.Localization.Abstractions;

namespace TrdP.Mvc.DataAnnotations.Localization.ClientModelValidators
{
    public abstract class ValidationAttributeAdapter<TAttribute> : IValidationAttributeAdapter
        where TAttribute : ValidationAttribute
    {
        private readonly IStringLocalizer _stringLocalizer;

        protected ValidationAttributeAdapter(TAttribute attribute, IStringLocalizer stringLocalizer)
        {
            Attribute = attribute;
            _stringLocalizer = stringLocalizer;
        }

        public TAttribute Attribute { get; }

        public abstract void AddValidation(ClientModelValidationContext context);

        protected static bool MergeAttribute(IDictionary<string, string> attributes, string key, string value)
        {
            if (attributes.ContainsKey(key))
            {
                return false;
            }

            attributes.Add(key, value);
            return true;
        }

        public abstract string GetErrorMessage(ModelValidationContextBase validationContext);

        protected virtual string GetErrorMessage(ModelMetadata modelMetadata, params object[] arguments)
        {
            if (modelMetadata == null)
            {
                throw new ArgumentNullException(nameof(modelMetadata));
            }

            return HasLocalizableErrorMessage()
                ? _stringLocalizer[Attribute.ErrorMessage, arguments]
                : Attribute.FormatErrorMessage(modelMetadata.GetDisplayName());
        }

        private bool HasLocalizableErrorMessage()
        {
            return _stringLocalizer != null &&
                   !string.IsNullOrEmpty(Attribute.ErrorMessage) &&
                   string.IsNullOrEmpty(Attribute.ErrorMessageResourceName) &&
                   Attribute.ErrorMessageResourceType == null;
        }
    }
}
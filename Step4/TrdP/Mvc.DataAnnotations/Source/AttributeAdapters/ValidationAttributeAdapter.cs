using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using TrdP.Localization.Abstractions;

namespace TrdP.Mvc.DataAnnotations.Localization.AttributeAdapters
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

        protected string GetLocalizedErrorMessage(params object[] arguments)
        {
            var defaultErrorMessage =
                (string)Attribute.GetType()
                    .GetProperty("ErrorMessageString", BindingFlags.NonPublic | BindingFlags.Instance)
                    ?.GetValue(Attribute);
            var errorMessage = Attribute.ErrorMessage ?? defaultErrorMessage;
            if (errorMessage == null)
            {
                return null;
            }
            if (Attribute.ErrorMessageResourceType != null || !string.IsNullOrEmpty(Attribute.ErrorMessageResourceName))
            {
                return string.Format(errorMessage, arguments);
            }

            return _stringLocalizer?[errorMessage, arguments] ?? string.Format(errorMessage, arguments);
        }
    }
}
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using TrdP.Localization.Abstractions;

namespace TrdP.Mvc.DataAnnotations.Localization.AttributeAdapters
{
    internal abstract class ValidationAttributeAdapter<TAttribute> : IValidationAttributeAdapter
        where TAttribute : ValidationAttribute
    {

        protected ValidationAttributeAdapter(TAttribute attribute, IStringLocalizer stringLocalizer)
        {
            Attribute = attribute ?? throw new ArgumentNullException(nameof(attribute));
            StringLocalizer = stringLocalizer ?? throw new ArgumentNullException(nameof(stringLocalizer));
        }

        protected TAttribute Attribute { get; }
        protected IStringLocalizer StringLocalizer { get; }

        public void AddValidation(ClientModelValidationContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            MergeAttribute(context.Attributes, "data-val", "true");
            AddAdapterValidation(context);
        }

        protected abstract void AddAdapterValidation(ClientModelValidationContext context);

        protected static void MergeAttribute(IDictionary<string, string> attributes, string key, string value)
        {
            if (attributes.ContainsKey(key))
            {
                return;
            }
            attributes.Add(key, value);
        }

        public string GetErrorMessage(ModelValidationContextBase context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            return GetAdapterErrorMessage(context);
        }

        protected abstract string GetAdapterErrorMessage(ModelValidationContextBase context);

        protected string GetLocalizedErrorMessage(params object[] arguments)
        {
            // ReSharper disable once PossibleNullReferenceException - Always exists
            var defaultErrorMessage = (string)Attribute.GetType()
                .GetProperty("ErrorMessageString", BindingFlags.NonPublic | BindingFlags.Instance)
                .GetValue(Attribute);
            var errorMessage = Attribute.ErrorMessage ?? defaultErrorMessage;
            return StringLocalizer[errorMessage, arguments];
        }
    }
}
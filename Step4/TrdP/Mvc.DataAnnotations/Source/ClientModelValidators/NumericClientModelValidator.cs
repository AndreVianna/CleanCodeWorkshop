using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace TrdP.Mvc.DataAnnotations.Localization.ClientModelValidators
{
    internal class NumericClientModelValidator : IClientModelValidator
    {
        public void AddValidation(ClientModelValidationContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            MergeAttribute(context.Attributes, "data-val", "true");
            MergeAttribute(context.Attributes, "data-val-number", GetErrorMessage(context.ModelMetadata));
        }

        private static void MergeAttribute(IDictionary<string, string> attributes, string key, string value)
        {
            if (!attributes.ContainsKey(key))
            {
                attributes.Add(key, value);
            }
        }

        private string GetErrorMessage(ModelMetadata modelMetadata)
        {
            if (modelMetadata == null)
            {
                throw new ArgumentNullException(nameof(modelMetadata));
            }

            var messageProvider = modelMetadata.ModelBindingMessageProvider;
            var name = modelMetadata.DisplayName ?? modelMetadata.Name;
            return name == null
                ? messageProvider.NonPropertyValueMustBeANumberAccessor()
                : messageProvider.ValueMustBeANumberAccessor(name);
        }
    }
}
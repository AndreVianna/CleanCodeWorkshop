using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using TrdP.Mvc.DataAnnotations.Localization.ClientModelValidators;
using TrdP.Mvc.DataAnnotations.Localization.ModelValidators;

namespace TrdP.Mvc.DataAnnotations.Localization.ValidationProviders
{
    internal class NumericClientModelValidatorProvider : IClientModelValidatorProvider
    {
        public void CreateValidators(ClientValidatorProviderContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            var typeToValidate = context.ModelMetadata.UnderlyingOrModelType;

            if (typeToValidate != typeof(float)
                && typeToValidate != typeof(double)
                && typeToValidate != typeof(decimal))
            {
                return;
            }

            if (context.Results.Any(result => result.Validator is NumericClientModelValidator))
            {
                return;
            }

            context.Results.Add(new ClientValidatorItem
            {
                Validator = new NumericClientModelValidator(),
                IsReusable = true
            });
        }
    }
}
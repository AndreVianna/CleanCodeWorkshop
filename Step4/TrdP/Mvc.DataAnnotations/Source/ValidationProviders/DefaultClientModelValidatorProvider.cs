using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace TrdP.Mvc.DataAnnotations.Localization.ValidationProviders
{
    internal class DefaultClientModelValidatorProvider : IClientModelValidatorProvider
    {
        public void CreateValidators(ClientValidatorProviderContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            var validatorItems = context.Results.Where(i => i.Validator == null).ToArray();
            foreach (var validatorItem in validatorItems)
            {
                if (!(validatorItem.ValidatorMetadata is IClientModelValidator validator))
                {
                    continue;
                }

                SetValidator(validatorItem, validator);
            }
        }

        private static void SetValidator(ClientValidatorItem validatorItem, IClientModelValidator validator)
        {
            validatorItem.Validator = validator;
            validatorItem.IsReusable = true;
        }
    }
}
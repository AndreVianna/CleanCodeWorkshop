using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace TrdP.Mvc.DataAnnotations.Localization.ValidationProviders
{
    internal class DefaultModelValidatorProvider : IModelValidatorProvider
    {
        public void CreateValidators(ModelValidatorProviderContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            var validatorItems = context.Results.Where(i => i.Validator == null).ToArray();
            foreach (var validatorItem in validatorItems)
            {
                if (!(validatorItem.ValidatorMetadata is IModelValidator validator))
                {
                    continue;
                }

                SetValidator(validatorItem, validator);
            }
        }

        private static void SetValidator(ValidatorItem validatorItem, IModelValidator validator)
        {
            validatorItem.Validator = validator;
            validatorItem.IsReusable = true;
        }
    }
}
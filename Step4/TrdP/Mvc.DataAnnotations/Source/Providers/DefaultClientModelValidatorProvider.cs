using System;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace TrdP.Mvc.DataAnnotations.Localization.Providers
{
    internal class DefaultClientModelValidatorProvider : IClientModelValidatorProvider
    {
        /// <inheritdoc />
        public void CreateValidators(ClientValidatorProviderContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            foreach (var validatorItem in context.Results)
            {
                // Don't overwrite anything that was done by a previous provider.
                if (validatorItem.Validator != null)
                {
                    continue;
                }

                if (!(validatorItem.ValidatorMetadata is IClientModelValidator validator))
                {
                    continue;
                }

                validatorItem.Validator = validator;
                validatorItem.IsReusable = true;
            }
        }
    }
}
using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using TrdP.Localization.Abstractions;
using TrdP.Mvc.DataAnnotations.Localization.ClientModelValidators;

namespace TrdP.Mvc.DataAnnotations.Localization.Providers
{
    internal class DataAnnotationsClientModelValidatorProvider : IClientModelValidatorProvider
    {
        private readonly IStringLocalizerFactory _stringLocalizerFactory;
        private readonly IValidationAttributeAdapterProvider _validationAttributeAdapterProvider;

        public DataAnnotationsClientModelValidatorProvider(
            IValidationAttributeAdapterProvider validationAttributeAdapterProvider,
            IStringLocalizerFactory stringLocalizerFactory)
        {
            _validationAttributeAdapterProvider = validationAttributeAdapterProvider ?? throw new ArgumentNullException(nameof(validationAttributeAdapterProvider));
            _stringLocalizerFactory = stringLocalizerFactory ?? throw new ArgumentNullException(nameof(stringLocalizerFactory));
        }

        public void CreateValidators(ClientValidatorProviderContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            var resourceSource = context.ModelMetadata.ContainerType ?? context.ModelMetadata.ModelType;
            var stringLocalizer = _stringLocalizerFactory.Create(resourceSource);

            var hasRequiredAttribute = false;
            foreach (var validatorItem in context.Results)
            {
                if (validatorItem.Validator != null)
                {
                    hasRequiredAttribute |= validatorItem.Validator is RequiredAttributeAdapter;
                    continue;
                }

                if (!(validatorItem.ValidatorMetadata is ValidationAttribute attribute))
                {
                    continue;
                }

                hasRequiredAttribute |= attribute is RequiredAttribute;
                var adapter = _validationAttributeAdapterProvider.GetAttributeAdapter(attribute, stringLocalizer);
                if (adapter == null)
                {
                    continue;
                }

                validatorItem.Validator = adapter;
                validatorItem.IsReusable = true;
            }

            if (hasRequiredAttribute || !context.ModelMetadata.IsRequired)
            {
                return;
            }

            context.Results.Add(new ClientValidatorItem
            {
                Validator = _validationAttributeAdapterProvider.GetAttributeAdapter(new RequiredAttribute(), stringLocalizer),
                IsReusable = true
            });
        }
    }
}
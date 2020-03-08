using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using TrdP.Localization.Abstractions;
using TrdP.Mvc.DataAnnotations.Localization.AttributeAdapters;

namespace TrdP.Mvc.DataAnnotations.Localization.ValidationProviders
{
    internal class ClientModelValidatorProvider : IClientModelValidatorProvider
    {
        private readonly IStringLocalizerFactory _stringLocalizerFactory;
        private readonly IValidationAttributeAdapterFactory _validationAttributeAdapterFactory;

        public ClientModelValidatorProvider(
            IValidationAttributeAdapterFactory validationAttributeAdapterFactory,
            IStringLocalizerFactory stringLocalizerFactory)
        {
            _validationAttributeAdapterFactory = validationAttributeAdapterFactory ?? throw new ArgumentNullException(nameof(validationAttributeAdapterFactory));
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

            var hasRequiredAttribute = context.Results.Any(i => i.Validator is RequiredAttributeAdapter);
            var validatorItems = context.Results.Where(i => i.Validator == null).ToArray();
            foreach (var validatorItem in validatorItems)
            {
                if (!(validatorItem.ValidatorMetadata is ValidationAttribute validationAttribute))
                {
                    continue;
                }

                SetValidator(validatorItem, validationAttribute, stringLocalizer);

                MoveRequiredAttributeToTop(context, validationAttribute, validatorItem);

                hasRequiredAttribute |= validationAttribute is RequiredAttribute;
            }

            EnsureHasRequiredAttribute(context, hasRequiredAttribute, stringLocalizer);
        }

        private static void MoveRequiredAttributeToTop(ClientValidatorProviderContext context, ValidationAttribute validationAttribute, ClientValidatorItem validatorItem)
        {
            if (!(validationAttribute is RequiredAttribute))
            {
                return;
            }

            context.Results.Remove(validatorItem);
            context.Results.Insert(0, validatorItem);
        }

        private void SetValidator(ClientValidatorItem validatorItem, ValidationAttribute validationAttribute, IStringLocalizer stringLocalizer)
        {
            validatorItem.Validator = _validationAttributeAdapterFactory.Create(validationAttribute, stringLocalizer);
            validatorItem.IsReusable = true;
        }

        private static void EnsureHasRequiredAttribute(ClientValidatorProviderContext context, bool hasRequiredAttribute,
            IStringLocalizer stringLocalizer)
        {
            if (!context.ModelMetadata.IsRequired || hasRequiredAttribute)
            {
                return;
            }

            var validatorItem = new ClientValidatorItem
            {
                Validator = new RequiredAttributeAdapter(new RequiredAttribute(), stringLocalizer),
                IsReusable = true
            };
            context.Results.Add(validatorItem);
        }
    }
}
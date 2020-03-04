using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using TrdP.Localization.Abstractions;
using TrdP.Mvc.DataAnnotations.Localization.AttributeAdapters;

namespace TrdP.Mvc.DataAnnotations.Localization.ValidationProviders
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

            var hasRequiredAttribute = context.Results.Any(i => i.Validator is RequiredAttributeAdapter);
            var validatorItems = context.Results.Where(i => i.Validator == null).ToArray();
            foreach (var validatorItem in validatorItems)
            {
                hasRequiredAttribute |= CreateValidator(context, validatorItem, stringLocalizer);
            }

            EnsureHasRequiredAttribute(context, hasRequiredAttribute, stringLocalizer);
        }

        private bool CreateValidator(ClientValidatorProviderContext context, ClientValidatorItem validatorItem, IStringLocalizer stringLocalizer)
        {
            if (!(validatorItem.ValidatorMetadata is ValidationAttribute validationAttribute))
            {
                return false;
            }

            SetValidator(validatorItem, validationAttribute, stringLocalizer);

            MoveRequiredAttributeToTop(context, validationAttribute, validatorItem);

            return validationAttribute is RequiredAttribute;
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
            validatorItem.Validator = _validationAttributeAdapterProvider.GetAttributeAdapter(validationAttribute, stringLocalizer);
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
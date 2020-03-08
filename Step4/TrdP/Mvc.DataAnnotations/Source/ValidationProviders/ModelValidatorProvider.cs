using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using TrdP.Localization.Abstractions;
using TrdP.Mvc.DataAnnotations.Localization.ModelValidators;

namespace TrdP.Mvc.DataAnnotations.Localization.ValidationProviders
{
    internal sealed class ModelValidatorProvider : IMetadataBasedModelValidatorProvider
    {
        private readonly IStringLocalizerFactory _stringLocalizerFactory;
        private readonly IValidationAttributeAdapterFactory _validationAttributeAdapterFactory;

        public ModelValidatorProvider(
            IValidationAttributeAdapterFactory validationAttributeAdapterFactory,
            IStringLocalizerFactory stringLocalizerFactory)
        {
            _validationAttributeAdapterFactory = validationAttributeAdapterFactory ?? throw new ArgumentNullException(nameof(validationAttributeAdapterFactory));
            _stringLocalizerFactory = stringLocalizerFactory ?? throw new ArgumentNullException(nameof(stringLocalizerFactory));
        }

        public bool HasValidators(Type modelType, IList<object> validatorMetadata)
        {
            return typeof(IValidatableObject).IsAssignableFrom(modelType) || validatorMetadata.OfType<ValidationAttribute>().Any();
        }

        public void CreateValidators(ModelValidatorProviderContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            var resourceSource = context.ModelMetadata.ContainerType ?? context.ModelMetadata.ModelType;
            var stringLocalizer = _stringLocalizerFactory.Create(resourceSource);

            var validatorItems = context.Results.Where(i => i.Validator == null).ToArray();
            foreach (var validatorItem in validatorItems)
            {
                if (!(validatorItem.ValidatorMetadata is ValidationAttribute validationAttribute))
                {
                    continue;
                }

                SetItemValidator(validatorItem, validationAttribute, stringLocalizer);

                MoveRequiredAttributeToTop(context, validationAttribute, validatorItem);
            }

            AddSupportToValidatableObject(context);
        }

        private static void AddSupportToValidatableObject(ModelValidatorProviderContext context)
        {
            if (typeof(IValidatableObject).IsAssignableFrom(context.ModelMetadata.ModelType))
            {
                context.Results.Add(new ValidatorItem
                {
                    Validator = new ValidatableObjectAdapter(),
                    IsReusable = true
                });
            }
        }

        private void SetItemValidator(ValidatorItem validatorItem, ValidationAttribute validationAttribute,
            IStringLocalizer stringLocalizer)
        {
            validatorItem.Validator =
                new ModelValidator(_validationAttributeAdapterFactory, validationAttribute, stringLocalizer);
            validatorItem.IsReusable = true;
        }

        private static void MoveRequiredAttributeToTop(ModelValidatorProviderContext context, ValidationAttribute validationAttribute, ValidatorItem validatorItem)
        {
            if (!(validationAttribute is RequiredAttribute))
            {
                return;
            }

            context.Results.Remove(validatorItem);
            context.Results.Insert(0, validatorItem);
        }
    }
}
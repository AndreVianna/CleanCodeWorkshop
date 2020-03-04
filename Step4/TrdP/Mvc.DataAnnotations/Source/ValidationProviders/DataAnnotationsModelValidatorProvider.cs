using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using TrdP.Localization.Abstractions;
using TrdP.Mvc.DataAnnotations.Localization.ModelValidators;

namespace TrdP.Mvc.DataAnnotations.Localization.ValidationProviders
{
    internal sealed class DataAnnotationsModelValidatorProvider : IMetadataBasedModelValidatorProvider
    {
        private readonly IStringLocalizerFactory _stringLocalizerFactory;
        private readonly IValidationAttributeAdapterProvider _validationAttributeAdapterProvider;

        public DataAnnotationsModelValidatorProvider(
            IValidationAttributeAdapterProvider validationAttributeAdapterProvider,
            IStringLocalizerFactory stringLocalizerFactory)
        {
            _validationAttributeAdapterProvider = validationAttributeAdapterProvider ?? throw new ArgumentNullException(nameof(validationAttributeAdapterProvider));
            _stringLocalizerFactory = stringLocalizerFactory ?? throw new ArgumentNullException(nameof(stringLocalizerFactory));
        }

        public void CreateValidators(ModelValidatorProviderContext context)
        {
            var resourceSource = context.ModelMetadata.ContainerType ?? context.ModelMetadata.ModelType;
            var stringLocalizer = _stringLocalizerFactory.Create(resourceSource);

            var validatorItems = context.Results.Where(i => i.Validator == null).ToArray();
            foreach (var validatorItem in validatorItems)
            {
                if (!(validatorItem.ValidatorMetadata is ValidationAttribute validationAttribute))
                {
                    continue;
                }

                SetValidator(validatorItem, validationAttribute, stringLocalizer);

                MoveRequiredAttributeToTop(context, validationAttribute, validatorItem);
            }

            // Produce a validator if the type supports IValidatableObject
            if (typeof(IValidatableObject).IsAssignableFrom(context.ModelMetadata.ModelType))
            {
                context.Results.Add(new ValidatorItem
                {
                    Validator = new ValidatableObjectAdapter(),
                    IsReusable = true
                });
            }
        }

        private void SetValidator(ValidatorItem validatorItem, ValidationAttribute validationAttribute,
            IStringLocalizer stringLocalizer)
        {
            validatorItem.Validator =
                new DataAnnotationsModelValidator(_validationAttributeAdapterProvider, validationAttribute, stringLocalizer);
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

        public bool HasValidators(Type modelType, IList<object> validatorMetadata)
        {
            if (typeof(IValidatableObject).IsAssignableFrom(modelType))
            {
                return true;
            }

            return validatorMetadata.OfType<ValidationAttribute>().Any();
        }
    }
}
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using TrdP.Localization.Abstractions;
using TrdP.Mvc.DataAnnotations.Localization.ClientModelValidators;

namespace TrdP.Mvc.DataAnnotations.Localization.Providers
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

            for (var i = 0; i < context.Results.Count; i++)
            {
                var validatorItem = context.Results[i];
                if (validatorItem.Validator != null)
                {
                    continue;
                }

                if (!(validatorItem.ValidatorMetadata is ValidationAttribute attribute))
                {
                    continue;
                }

                var validator = new DataAnnotationsModelValidator(
                    _validationAttributeAdapterProvider,
                    attribute,
                    stringLocalizer);

                validatorItem.Validator = validator;
                validatorItem.IsReusable = true;
                // Inserts validators based on whether or not they are 'required'. We want to run
                // 'required' validators first so that we get the best possible error message.
                if (attribute is RequiredAttribute)
                {
                    context.Results.Remove(validatorItem);
                    context.Results.Insert(0, validatorItem);
                }
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

        public bool HasValidators(Type modelType, IList<object> validatorMetadata)
        {
            if (typeof(IValidatableObject).IsAssignableFrom(modelType))
            {
                return true;
            }

            for (var i = 0; i < validatorMetadata.Count; i++)
            {
                if (validatorMetadata[i] is ValidationAttribute)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
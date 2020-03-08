using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using TrdP.Localization.Abstractions;
using TrdP.Mvc.DataAnnotations.Localization.ValidationProviders;
using static TrdP.Mvc.DataAnnotations.Localization.Helpers.ModelValidationResultHelper;

namespace TrdP.Mvc.DataAnnotations.Localization.ModelValidators
{
    internal class ModelValidator : IModelValidator
    {
        private static readonly object _emptyValidationContextInstance = new object();
        private readonly IStringLocalizer _stringLocalizer;
        private readonly IValidationAttributeAdapterFactory _validationAttributeAdapterFactory;

        public ModelValidator(
            IValidationAttributeAdapterFactory validationAttributeAdapterFactory,
            ValidationAttribute attribute,
            IStringLocalizer stringLocalizer = null)
        {
            _validationAttributeAdapterFactory = validationAttributeAdapterFactory ?? throw new ArgumentNullException(nameof(validationAttributeAdapterFactory));
            Attribute = attribute ?? throw new ArgumentNullException(nameof(attribute));
            _stringLocalizer = stringLocalizer;
        }

        public ValidationAttribute Attribute { get; }

        public IEnumerable<ModelValidationResult> Validate(ModelValidationContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            var subject = context.Container ?? context.Model ?? _emptyValidationContextInstance;
            var serviceProvider = context.ActionContext.HttpContext?.RequestServices;

            var validationContext = new ValidationContext(subject, serviceProvider, null)
            {
                DisplayName = context.ModelMetadata.GetDisplayName(),
                MemberName = context.ModelMetadata.Name,
            };

            var validationResult = Attribute.GetValidationResult(context.Model, validationContext);
            var memberName = context.ModelMetadata.Name;
            var errorMessage = GetErrorMessage(context);
            return GenerateModelValidationResults(validationResult, errorMessage, memberName);
        }

        private string GetErrorMessage(ModelValidationContextBase validationContext)
        {
            var adapter = _validationAttributeAdapterFactory.Create(Attribute, _stringLocalizer);
            return adapter.GetErrorMessage(validationContext);
        }
    }
}
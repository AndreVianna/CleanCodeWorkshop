using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using static TrdP.Mvc.DataAnnotations.Localization.Helpers.ModelValidationResultHelper;

namespace TrdP.Mvc.DataAnnotations.Localization.ModelValidators
{
    internal class ValidatableObjectAdapter : IModelValidator
    {
        public IEnumerable<ModelValidationResult> Validate(ModelValidationContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            var model = context.Model;
            if (model == null)
            {
                return NoResults();
            }

            if (!(model is IValidatableObject subject))
            {
                var message = $"The model object inside the metadata claimed to be compatible with '{typeof(IValidatableObject).Name}', but was actually '{model.GetType()}'.";
                throw new InvalidOperationException(message);
            }
            var serviceProvider = context.ActionContext.HttpContext?.RequestServices;

            var validationContext = new ValidationContext(subject, serviceProvider, null)
            {
                DisplayName = context.ModelMetadata.GetDisplayName(),
                MemberName = context.ModelMetadata.Name,
            };
            var validationResult = subject.Validate(validationContext);

            return GenerateModelValidationResults(validationResult);
        }
    }
}
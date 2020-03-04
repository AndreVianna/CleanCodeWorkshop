using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using TrdP.Localization.Abstractions;
using TrdP.Mvc.DataAnnotations.Localization.ValidationProviders;

namespace TrdP.Mvc.DataAnnotations.Localization.ModelValidators
{
    internal class DataAnnotationsModelValidator : IModelValidator
    {
        private static readonly object _emptyValidationContextInstance = new object();
        private readonly IStringLocalizer _stringLocalizer;
        private readonly IValidationAttributeAdapterProvider _validationAttributeAdapterProvider;

        public DataAnnotationsModelValidator(
            IValidationAttributeAdapterProvider validationAttributeAdapterProvider,
            ValidationAttribute attribute,
            IStringLocalizer stringLocalizer)
        {
            _validationAttributeAdapterProvider = validationAttributeAdapterProvider ?? throw new ArgumentNullException(nameof(validationAttributeAdapterProvider));
            Attribute = attribute ?? throw new ArgumentNullException(nameof(attribute));
            _stringLocalizer = stringLocalizer;
        }

        public ValidationAttribute Attribute { get; }

        public IEnumerable<ModelValidationResult> Validate(ModelValidationContext validationContext)
        {
            if (validationContext == null)
            {
                throw new ArgumentNullException(nameof(validationContext));
            }
            if (validationContext.ModelMetadata == null)
            {
                throw new ArgumentException(
                    $"The '{nameof(validationContext.ModelMetadata)}' property of '{typeof(ModelValidationContext)}' must not be null.",
                    nameof(validationContext));
            }
            if (validationContext.MetadataProvider == null)
            {
                throw new ArgumentException(
                    $"The '{nameof(validationContext.MetadataProvider)}' property of '{typeof(ModelValidationContext)}' must not be null.",
                    nameof(validationContext));
            }

            var metadata = validationContext.ModelMetadata;
            var memberName = metadata.Name;
            var container = validationContext.Container;

            var context = new ValidationContext(
                instance: container ?? validationContext.Model ?? _emptyValidationContextInstance,
                serviceProvider: validationContext.ActionContext?.HttpContext?.RequestServices,
                items: null)
            {
                DisplayName = metadata.GetDisplayName(),
                MemberName = memberName
            };

            var result = Attribute.GetValidationResult(validationContext.Model, context);
            if (result == null || result == ValidationResult.Success)
            {
                return Enumerable.Empty<ModelValidationResult>();
            }

            var errorMessage = GetErrorMessage(validationContext) ?? result.ErrorMessage;

            var validationResults = new List<ModelValidationResult>();
            if (result.MemberNames != null)
            {
                foreach (var resultMemberName in result.MemberNames)
                {
                    // ModelValidationResult.MemberName is used by invoking validators (such as ModelValidator) to
                    // append construct the ModelKey for ModelStateDictionary. When validating at type level we
                    // want the returned MemberNames if specified (e.g. "person.Address.FirstName"). For property
                    // validation, the ModelKey can be constructed using the ModelMetadata and we should ignore
                    // MemberName (we don't want "person.Name.Name"). However the invoking validator does not have
                    // a way to distinguish between these two cases. Consequently we'll only set MemberName if this
                    // validation returns a MemberName that is different from the property being validated.
                    var validationResult = new ModelValidationResult(GetValidationResultMemberName(resultMemberName), errorMessage);

                    validationResults.Add(validationResult);
                }
            }

            if (validationResults.Count == 0)
            {
                // result.MemberNames was null or empty.
                validationResults.Add(new ModelValidationResult(memberName: null, message: errorMessage));
            }

            return validationResults;

            string GetValidationResultMemberName(string resultMemberName)
            {
                return resultMemberName.Equals(memberName, StringComparison.Ordinal)
                    ? null
                    : resultMemberName;
            }
        }

        private string GetErrorMessage(ModelValidationContextBase validationContext)
        {
            var adapter = _validationAttributeAdapterProvider.GetAttributeAdapter(Attribute, _stringLocalizer);
            return adapter?.GetErrorMessage(validationContext);
        }
    }
}
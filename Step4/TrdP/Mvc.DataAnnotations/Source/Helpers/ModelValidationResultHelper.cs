using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace TrdP.Mvc.DataAnnotations.Localization.Helpers
{
    internal static class ModelValidationResultHelper
    {
        public static IEnumerable<ModelValidationResult> NoResults()
        {
            return Enumerable.Empty<ModelValidationResult>();
        }

        private static IEnumerable<ModelValidationResult> SingleResult(string memberName, string errorMessage)
        {
            return new[] { new ModelValidationResult(memberName: memberName, message: errorMessage) };
        }

        public static IEnumerable<ModelValidationResult> GenerateModelValidationResults(IEnumerable<ValidationResult> validationResults)
        {
            if (validationResults == null)
            {
                return NoResults();
            }

            var results = new List<ModelValidationResult>();
            foreach (var validationResult in validationResults)
            {
                results.AddRange(GenerateModelValidationResults(validationResult));
            }
            return results;
        }

        private static IEnumerable<ModelValidationResult> GenerateModelValidationResults(ValidationResult validationResult)
        {
            return GenerateModelValidationResults(validationResult, validationResult?.ErrorMessage, null);
        }

        public static IEnumerable<ModelValidationResult> GenerateModelValidationResults(ValidationResult validationResult, string errorMessage, string memberName)
        {
            if (validationResult == null || validationResult == ValidationResult.Success)
            {
                return NoResults();
            }

            if (!validationResult.MemberNames.Any())
            {
                return SingleResult(null, errorMessage);
            }

            return from candidate
                    in validationResult.MemberNames
                select new ModelValidationResult(GetFinalValidationResultMemberName(candidate, memberName), errorMessage);
        }

        private static string GetFinalValidationResultMemberName(string candidate, string memberName)
        {
            return candidate == null
                ? memberName
                : !candidate.Equals(memberName, StringComparison.Ordinal)
                    ? candidate
                    : null;
        }
    }
}
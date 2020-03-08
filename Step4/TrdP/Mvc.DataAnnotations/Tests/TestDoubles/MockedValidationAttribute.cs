using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TrdP.Mvc.DataAnnotations.Localization.Tests.TestDoubles
{
    internal sealed class MockedValidationAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            return new ValidationResult("ErrorMassage", new List<string>{ "TestProperty", "OtherProperty" });
        }
    }
}
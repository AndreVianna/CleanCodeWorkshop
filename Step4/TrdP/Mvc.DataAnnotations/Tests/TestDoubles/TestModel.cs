using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TrdP.Mvc.DataAnnotations.Localization.Tests.TestDoubles
{
    internal class TestModel
    {
        public string TestProperty { get; set; }

        public string OtherProperty => "OtherValue";
    }

    internal class TestValidatableObject : IValidatableObject
    {
        private readonly IEnumerable<ValidationResult> _expectedValidationResults;

        public TestValidatableObject(IEnumerable<ValidationResult> expectedValidationResults)
        {
            _expectedValidationResults = expectedValidationResults;
        }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext) => _expectedValidationResults;
    }
}
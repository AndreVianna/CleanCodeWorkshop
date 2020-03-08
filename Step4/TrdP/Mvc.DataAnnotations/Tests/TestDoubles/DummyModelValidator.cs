using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace TrdP.Mvc.DataAnnotations.Localization.Tests.TestDoubles
{
    public class DummyModelValidator : IModelValidator
    {
        public IEnumerable<ModelValidationResult> Validate(ModelValidationContext context) => throw new NotImplementedException();
    }
}
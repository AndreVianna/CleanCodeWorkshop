using System;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace TrdP.Mvc.DataAnnotations.Localization.Tests.TestDoubles
{
    public class DummyClientModelValidator : IClientModelValidator
    {
        public void AddValidation(ClientModelValidationContext context) => throw new NotImplementedException();
    }
}
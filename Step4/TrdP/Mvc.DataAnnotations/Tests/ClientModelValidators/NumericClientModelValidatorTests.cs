using System;
using TrdP.Mvc.DataAnnotations.Localization.ClientModelValidators;
using TrdP.Mvc.DataAnnotations.Localization.Tests.Helpers;
using Xunit;

namespace TrdP.Mvc.DataAnnotations.Localization.Tests.ClientModelValidators
{
    public class NumericClientModelValidatorTests
    {
        private readonly NumericClientModelValidator _adapter;

        public NumericClientModelValidatorTests()
        {
            _adapter = new NumericClientModelValidator();
        }

        [Fact]
        public void ValidationAttributeAdapter_AddValidation_WithNullContext_ShouldThrow()
        {
            Assert.Throws<ArgumentNullException>(() => _adapter.AddValidation(null));
        }


        [Fact]
        public void ValidationAttributeAdapter_AddValidation_ShouldPass()
        {
            var context = ClientModelValidationContextFactory.CreateForProperty();
            _adapter.AddValidation(context);
            Assert.Contains("data-val", context.Attributes);
            Assert.Contains("data-val-number", context.Attributes);
            Assert.Contains("The field TestProperty must be a number.", context.Attributes["data-val-number"]);
        }

        [Fact]
        public void ValidationAttributeAdapter_AddValidation_WithDisplayName_ShouldPass()
        {
            var context = ClientModelValidationContextFactory.CreateForType();
            _adapter.AddValidation(context);
            Assert.Contains("data-val", context.Attributes);
            Assert.Contains("data-val-number", context.Attributes);
            Assert.Contains("The field must be a number.", context.Attributes["data-val-number"]);
        }

        [Fact]
        public void ValidationAttributeAdapter_AddValidationCalledTwice_ShouldPass()
        {
            var context = ClientModelValidationContextFactory.CreateForProperty();
            _adapter.AddValidation(context);
            _adapter.AddValidation(context);
        }
    }
}
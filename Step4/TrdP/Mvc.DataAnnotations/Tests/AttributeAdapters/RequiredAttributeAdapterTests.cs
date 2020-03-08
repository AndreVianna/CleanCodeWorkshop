using System.ComponentModel.DataAnnotations;
using TrdP.Mvc.DataAnnotations.Localization.AttributeAdapters;
using TrdP.Mvc.DataAnnotations.Localization.Tests.Helpers;
using TrdP.Mvc.DataAnnotations.Localization.Tests.TestDoubles;
using Xunit;

namespace TrdP.Mvc.DataAnnotations.Localization.Tests.AttributeAdapters
{
    public class RequiredAttributeAdapterTests
    {
        private readonly RequiredAttributeAdapter _adapter;

        public RequiredAttributeAdapterTests()
        {
            var attribute = new RequiredAttribute();
            var stringLocalizer = new FakeStringLocalizer();
            _adapter = new RequiredAttributeAdapter(attribute, stringLocalizer);
        }

        [Fact]
        public void RequiredAttributeAdapter_AddValidation_ShouldPass()
        {
            var context = ClientModelValidationContextFactory.CreateForProperty();
            _adapter.AddValidation(context);
            Assert.Contains("data-val", context.Attributes);
            Assert.Contains("data-val-required", context.Attributes);
        }

        [Fact]
        public void RequiredAttributeAdapter_GetErrorMessage_ShouldPass()
        {
            var context = ClientModelValidationContextFactory.CreateForProperty();
            var result = _adapter.GetErrorMessage(context);
            Assert.Equal("The TestProperty field is required.", result);
        }
    }
}
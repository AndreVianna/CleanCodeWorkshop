using System.ComponentModel.DataAnnotations;
using TrdP.Mvc.DataAnnotations.Localization.AttributeAdapters;
using TrdP.Mvc.DataAnnotations.Localization.Tests.Helpers;
using TrdP.Mvc.DataAnnotations.Localization.Tests.TestDoubles;
using Xunit;

namespace TrdP.Mvc.DataAnnotations.Localization.Tests.AttributeAdapters
{
    public class CompareAttributeAdapterTests
    {
        private readonly CompareAttributeAdapter _adapter;

        public CompareAttributeAdapterTests()
        {
            var attribute = new CompareAttribute("OtherProperty");
            var stringLocalizer = new FakeStringLocalizer();
            _adapter = new CompareAttributeAdapter(attribute, stringLocalizer);
        }

        [Fact]
        public void CompareAttributeAdapter_AddValidation_ShouldPass()
        {
            var context = ClientModelValidationContextFactory.CreateForProperty();
            _adapter.AddValidation(context);
            Assert.Contains("data-val", context.Attributes);
            Assert.Contains("data-val-equalto", context.Attributes);
            Assert.Contains("data-val-equalto-other", context.Attributes);
        }

        [Fact]
        public void CompareAttributeAdapter_GetErrorMessage_ShouldPass()
        {
            var context = ClientModelValidationContextFactory.CreateForProperty();
            var result = _adapter.GetErrorMessage(context);
            Assert.Equal("'TestProperty' and 'OtherProperty' do not match.", result);
        }

        [Fact]
        public void CompareAttributeAdapter_GetErrorMessage_WithDisplayNames_ShouldPass()
        {
            var context = ClientModelValidationContextFactory.CreateForPropertyWithDisplayName();
            var result = _adapter.GetErrorMessage(context);
            Assert.Equal("'Named' and 'Other' do not match.", result);
        }
    }
}
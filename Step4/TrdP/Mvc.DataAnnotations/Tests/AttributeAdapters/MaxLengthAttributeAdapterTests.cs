using System.ComponentModel.DataAnnotations;
using TrdP.Mvc.DataAnnotations.Localization.AttributeAdapters;
using TrdP.Mvc.DataAnnotations.Localization.Tests.Helpers;
using TrdP.Mvc.DataAnnotations.Localization.Tests.TestDoubles;
using Xunit;

namespace TrdP.Mvc.DataAnnotations.Localization.Tests.AttributeAdapters
{
    public class MaxLengthAttributeAdapterTests
    {
        private readonly MaxLengthAttributeAdapter _adapter;

        public MaxLengthAttributeAdapterTests()
        {
            var attribute = new MaxLengthAttribute(10);
            var stringLocalizer = new FakeStringLocalizer();
            _adapter = new MaxLengthAttributeAdapter(attribute, stringLocalizer);
        }

        [Fact]
        public void MaxLengthAttributeAdapter_AddValidation_ShouldPass()
        {
            var context = ClientModelValidationContextFactory.CreateForProperty();
            _adapter.AddValidation(context);
            Assert.Contains("data-val", context.Attributes);
            Assert.Contains("data-val-maxlength", context.Attributes);
            Assert.Contains("data-val-maxlength-max", context.Attributes);
        }

        [Fact]
        public void MaxLengthAttributeAdapter_GetErrorMessage_ShouldPass()
        {
            var context = ClientModelValidationContextFactory.CreateForProperty();
            var result = _adapter.GetErrorMessage(context);
            Assert.Equal("The field TestProperty must be a string or array type with a maximum length of '10'.", result);
        }
    }
}
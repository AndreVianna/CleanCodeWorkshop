using System.ComponentModel.DataAnnotations;
using TrdP.Mvc.DataAnnotations.Localization.AttributeAdapters;
using TrdP.Mvc.DataAnnotations.Localization.Tests.Helpers;
using TrdP.Mvc.DataAnnotations.Localization.Tests.TestDoubles;
using Xunit;

namespace TrdP.Mvc.DataAnnotations.Localization.Tests.AttributeAdapters
{
    public class MinLengthAttributeAdapterTests
    {
        private readonly MinLengthAttributeAdapter _adapter;

        public MinLengthAttributeAdapterTests()
        {
            var attribute = new MinLengthAttribute(10);
            var stringLocalizer = new FakeStringLocalizer();
            _adapter = new MinLengthAttributeAdapter(attribute, stringLocalizer);
        }

        [Fact]
        public void MinLengthAttributeAdapter_AddValidation_ShouldPass()
        {
            var context = ClientModelValidationContextFactory.CreateForProperty();
            _adapter.AddValidation(context);
            Assert.Contains("data-val", context.Attributes);
            Assert.Contains("data-val-minlength", context.Attributes);
            Assert.Contains("data-val-minlength-min", context.Attributes);
        }

        [Fact]
        public void MinLengthAttributeAdapter_GetErrorMessage_ShouldPass()
        {
            var context = ClientModelValidationContextFactory.CreateForProperty();
            var result = _adapter.GetErrorMessage(context);
            Assert.Equal("The field TestProperty must be a string or array type with a minimum length of '10'.", result);
        }
    }
}
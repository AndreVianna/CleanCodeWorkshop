using System.ComponentModel.DataAnnotations;
using TrdP.Mvc.DataAnnotations.Localization.AttributeAdapters;
using TrdP.Mvc.DataAnnotations.Localization.Tests.Helpers;
using TrdP.Mvc.DataAnnotations.Localization.Tests.TestDoubles;
using Xunit;

namespace TrdP.Mvc.DataAnnotations.Localization.Tests.AttributeAdapters
{
    public class StringLengthAttributeAdapterTests
    {
        [Fact]
        public void StringLengthAttributeAdapter_AddValidation_ShouldPass()
        {
            var adapter = CreateAdapter(10);
            var context = ClientModelValidationContextFactory.CreateForProperty();
            adapter.AddValidation(context);
            Assert.Contains("data-val", context.Attributes);
            Assert.Contains("data-val-length", context.Attributes);
            Assert.Contains("data-val-length-max", context.Attributes);
        }

        [Fact]
        public void StringLengthAttributeAdapter_AddValidation_WithMinLength_ShouldPass()
        {
            var adapter = CreateAdapter(10, 2);
            var context = ClientModelValidationContextFactory.CreateForProperty();
            adapter.AddValidation(context);
            Assert.Contains("data-val", context.Attributes);
            Assert.Contains("data-val-length", context.Attributes);
            Assert.Contains("data-val-length-max", context.Attributes);
            Assert.Contains("data-val-length-min", context.Attributes);
        }

        [Fact]
        public void StringLengthAttributeAdapter_GetErrorMessage_ShouldPass()
        {
            var adapter = CreateAdapter(10);
            var context = ClientModelValidationContextFactory.CreateForProperty();
            var result = adapter.GetErrorMessage(context);
            Assert.Equal("The field TestProperty must be a string with a maximum length of 10.", result);
        }

        [Fact]
        public void StringLengthAttributeAdapter_GetErrorMessage_WithMinLength_ShouldPass()
        {
            var adapter = CreateAdapter(10, 2);
            var context = ClientModelValidationContextFactory.CreateForProperty();
            var result = adapter.GetErrorMessage(context);
            Assert.Equal("The field TestProperty must be a string with a minimum length of 2 and a maximum length of 10.", result);
        }

        private static StringLengthAttributeAdapter CreateAdapter(int maxLength, int? minLength = null)
        {
            var attribute = new StringLengthAttribute(maxLength);
            attribute.MinimumLength = minLength ?? attribute.MinimumLength;
            var stringLocalizer = new FakeStringLocalizer();
            return new StringLengthAttributeAdapter(attribute, stringLocalizer);
        }
    }
}
using System.ComponentModel.DataAnnotations;
using TrdP.Mvc.DataAnnotations.Localization.AttributeAdapters;
using TrdP.Mvc.DataAnnotations.Localization.Tests.Helpers;
using TrdP.Mvc.DataAnnotations.Localization.Tests.TestDoubles;
using Xunit;

namespace TrdP.Mvc.DataAnnotations.Localization.Tests.AttributeAdapters
{
    public class RegularExpressionAttributeAdapterTests
    {
        private readonly RegularExpressionAttributeAdapter _adapter;

        public RegularExpressionAttributeAdapterTests()
        {
            var attribute = new RegularExpressionAttribute("^[0-9]+$");
            var stringLocalizer = new FakeStringLocalizer();
            _adapter = new RegularExpressionAttributeAdapter(attribute, stringLocalizer);
        }

        [Fact]
        public void RegularExpressionAttributeAdapter_AddValidation_ShouldPass()
        {
            var context = ClientModelValidationContextFactory.CreateForProperty();
            _adapter.AddValidation(context);
            Assert.Contains("data-val", context.Attributes);
            Assert.Contains("data-val-regex", context.Attributes);
            Assert.Contains("data-val-regex-pattern", context.Attributes);
        }

        [Fact]
        public void RegularExpressionAttributeAdapter_GetErrorMessage_ShouldPass()
        {
            var context = ClientModelValidationContextFactory.CreateForProperty();
            var result = _adapter.GetErrorMessage(context);
            Assert.Equal("The field TestProperty must match the regular expression '^[0-9]+$'.", result);
        }
    }
}
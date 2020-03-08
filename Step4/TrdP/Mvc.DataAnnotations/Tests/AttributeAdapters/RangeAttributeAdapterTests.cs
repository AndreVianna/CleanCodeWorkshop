using System.ComponentModel.DataAnnotations;
using TrdP.Mvc.DataAnnotations.Localization.AttributeAdapters;
using TrdP.Mvc.DataAnnotations.Localization.Tests.Helpers;
using TrdP.Mvc.DataAnnotations.Localization.Tests.TestDoubles;
using Xunit;

namespace TrdP.Mvc.DataAnnotations.Localization.Tests.AttributeAdapters
{
    public class RangeAttributeAdapterTests
    {
        private readonly RangeAttributeAdapter _adapter;

        public RangeAttributeAdapterTests()
        {
            var attribute = new RangeAttribute(3, 10);
            var stringLocalizer = new FakeStringLocalizer();
            _adapter = new RangeAttributeAdapter(attribute, stringLocalizer);
        }

        [Fact]
        public void RangeAttributeAdapter_AddValidation_ShouldPass()
        {
            var context = ClientModelValidationContextFactory.CreateForProperty();
            _adapter.AddValidation(context);
            Assert.Contains("data-val", context.Attributes);
            Assert.Contains("data-val-range", context.Attributes);
            Assert.Contains("data-val-range-max", context.Attributes);
            Assert.Contains("data-val-range-min", context.Attributes);
        }

        [Fact]
        public void RangeAttributeAdapter_GetErrorMessage_ShouldPass()
        {
            var context = ClientModelValidationContextFactory.CreateForProperty();
            var result = _adapter.GetErrorMessage(context);
            Assert.Equal("The field TestProperty must be between 3 and 10.", result);
        }
    }
}
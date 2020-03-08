using System;
using System.ComponentModel.DataAnnotations;
using TrdP.Mvc.DataAnnotations.Localization.AttributeAdapters;
using TrdP.Mvc.DataAnnotations.Localization.Tests.Helpers;
using TrdP.Mvc.DataAnnotations.Localization.Tests.TestDoubles;
using Xunit;

namespace TrdP.Mvc.DataAnnotations.Localization.Tests.AttributeAdapters
{
    public class DataTypeAttributeAdapterTests
    {
        private readonly DataTypeAttributeAdapter _adapter;

        public DataTypeAttributeAdapterTests()
        {
            var attribute = new DataTypeAttribute("CustomDataType");
            var stringLocalizer = new FakeStringLocalizer();
            _adapter = new DataTypeAttributeAdapter(attribute, "custom", stringLocalizer);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("  ")]
        public void DataTypeAttributeAdapter_Constructor_WithInvalidRuleName_ShouldThrow(string ruleName)
        {
            Assert.Throws<ArgumentException>(() => new DataTypeAttributeAdapter(new DataTypeAttribute("CustomDataType"), ruleName, new FakeStringLocalizer()));
        }

        [Fact]
        public void DataTypeAttributeAdapter_AddValidation_ShouldPass()
        {
            var context = ClientModelValidationContextFactory.CreateForProperty();
            _adapter.AddValidation(context);
            Assert.Contains("data-val", context.Attributes);
            Assert.Contains("data-val-custom", context.Attributes);
        }

        [Fact]
        public void DataTypeAttributeAdapter_GetErrorMessage_ShouldPass()
        {
            var context = ClientModelValidationContextFactory.CreateForProperty();
            var result = _adapter.GetErrorMessage(context);
            Assert.Equal("The field TestProperty is invalid.", result);
        }
    }
}
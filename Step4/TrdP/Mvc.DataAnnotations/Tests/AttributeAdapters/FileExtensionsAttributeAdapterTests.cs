using System.ComponentModel.DataAnnotations;
using TrdP.Mvc.DataAnnotations.Localization.AttributeAdapters;
using TrdP.Mvc.DataAnnotations.Localization.Tests.Helpers;
using TrdP.Mvc.DataAnnotations.Localization.Tests.TestDoubles;
using Xunit;

namespace TrdP.Mvc.DataAnnotations.Localization.Tests.AttributeAdapters
{
    public class FileExtensionsAttributeAdapterTests
    {
        private readonly FileExtensionsAttributeAdapter _adapter;

        public FileExtensionsAttributeAdapterTests()
        {
            var attribute = new FileExtensionsAttribute();
            var stringLocalizer = new FakeStringLocalizer();
            _adapter = new FileExtensionsAttributeAdapter(attribute, stringLocalizer);
        }

        [Fact]
        public void FileExtensionsAttributeAdapter_AddValidation_ShouldPass()
        {
            var context = ClientModelValidationContextFactory.CreateForProperty();
            _adapter.AddValidation(context);
            Assert.Contains("data-val", context.Attributes);
            Assert.Contains("data-val-fileextensions", context.Attributes);
            Assert.Contains("data-val-fileextensions-extensions", context.Attributes);
        }

        [Fact]
        public void FileExtensionsAttributeAdapter_GetErrorMessage_ShouldPass()
        {
            var context = ClientModelValidationContextFactory.CreateForProperty();
            var result = _adapter.GetErrorMessage(context);
            Assert.Equal("The TestProperty field only accepts files with the following extensions: .png, .jpg, .jpeg, .gif", result);
        }
    }
}
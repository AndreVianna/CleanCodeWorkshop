using System;
using System.Globalization;
using NSubstitute;
using TrdP.Localization.Abstractions;
using TrdP.Mvc.Localization.Abstractions;
using TrdP.UnitTestsResources;
using Xunit;

namespace TrdP.Mvc.Localization.Tests
{
    public class HtmlLocalizerOfResourcesLocatorTests
    {
        private const string SOURCE_ASSEMBLY_NAME = "TrdP.Localization.TestData";
        private readonly IStringLocalizer _mockedStringLocalizer;
        private readonly IHtmlLocalizerFactory _mockedHtmlLocalizerFactory;

        public HtmlLocalizerOfResourcesLocatorTests()
        {
            _mockedStringLocalizer = Substitute.For<IStringLocalizer>();
            _mockedStringLocalizer["NotFound"].Returns(new LocalizedString("NotFound", "NotFound", true, BuildExpectedSearchedPath()));
            _mockedStringLocalizer["TestString"].Returns(new LocalizedString("TestString", "StringValue", false, BuildExpectedSearchedPath()));
            _mockedStringLocalizer["<b>TestHtml</b>"].Returns(new LocalizedString("<b>TestHtml</b>", "<b>HtmlValue</b>", false, BuildExpectedSearchedPath()));
            _mockedStringLocalizer["<b>FormattedHtml {0}</b>"].Returns(new LocalizedString("<b>FormattedHtml {0}</b>", "<b>FormattedValue {0}</b>", false, BuildExpectedSearchedPath()));

            _mockedHtmlLocalizerFactory = Substitute.For<IHtmlLocalizerFactory>();
            _mockedHtmlLocalizerFactory.Create<TestResources>().Returns(new HtmlLocalizer(_mockedStringLocalizer));
        }

        [Fact]
        public void HtmlLocalizer_Constructor_ShouldPass()
        {
            var _ = new HtmlLocalizer<TestResources>(_mockedHtmlLocalizerFactory);
        }

        [Fact]
        public void HtmlLocalizer_Constructor_WithNullStringLocalizerFactory_ShouldThrow()
        {
            Assert.Throws<ArgumentNullException>(() => new HtmlLocalizer<TestResources>(null));
        }

        [Theory]
        [InlineData("NotFound", "NotFound", true)]
        [InlineData("TestString", "StringValue", false)]
        [InlineData("<b>TestHtml</b>", "<b>HtmlValue</b>", false)]
        public void HtmlLocalizer_This_ShouldPass(string resourceName, string expectedValue, bool expectedNotToBeFound)
        {
            var localizer = new HtmlLocalizer<TestResources>(_mockedHtmlLocalizerFactory);
            var subject = localizer[resourceName];
            var _ = _mockedStringLocalizer.Received()[resourceName];
            AssertLocalizedHtmlContentResult(subject, resourceName, expectedValue, expectedNotToBeFound);
        }

        [Fact]
        public void HtmlLocalizer_This_WithArguments_ShouldPass()
        {
            var localizer = new HtmlLocalizer<TestResources>(_mockedHtmlLocalizerFactory);
            var subject = localizer["<b>FormattedHtml {0}</b>", 3];
            var _ = _mockedStringLocalizer.Received()["<b>FormattedHtml {0}</b>"];
            AssertLocalizedHtmlContentResult(subject, "<b>FormattedHtml {0}</b>", "<b>FormattedValue {0}</b>", false);
        }

        // ReSharper disable ParameterOnlyUsedForPreconditionCheck.Local
        private void AssertLocalizedHtmlContentResult(LocalizedHtmlContent result, string resourceName, string expectedValue,
            bool expectedFound)
        {
            Assert.Equal(resourceName, result.Name);
            Assert.Equal(expectedValue, result.Value);
            Assert.Equal(expectedFound, result.ResourceWasNotFound);
            Assert.Equal(BuildExpectedSearchedPath(), result.SearchedLocation);
        }
        // ReSharper restore ParameterOnlyUsedForPreconditionCheck.Local

        private static string BuildExpectedSearchedPath()
        {
            return $"{SOURCE_ASSEMBLY_NAME}.Resources.{CultureInfo.CurrentUICulture.Name}.resx";
        }
    }
}

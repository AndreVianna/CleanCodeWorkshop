using System;
using System.Globalization;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures.Internal;
using NSubstitute;
using TrdP.Localization.Abstractions;
using TrdP.Localization.Mvc.Abstractions;
using Xunit;

namespace TrdP.Localization.Mvc.Tests
{
    public class ViewLocalizerTests
    {
        private const string SOURCE_ASSEMBLY_NAME = "TrdP.Localization.TestData";
        private readonly IHtmlLocalizer _mockedHtmlLocalizer;

        public ViewLocalizerTests()
        {
            _mockedHtmlLocalizer = Substitute.For<IHtmlLocalizer>();
            _mockedHtmlLocalizer["NotFound"].Returns(new LocalizedHtmlContent("NotFound", "NotFound", true, BuildExpectedSearchedPath()));
            _mockedHtmlLocalizer["TestString"].Returns(new LocalizedHtmlContent("TestString", "StringValue", false, BuildExpectedSearchedPath()));
            _mockedHtmlLocalizer["<b>TestHtml</b>", 3].Returns(new LocalizedHtmlContent("<b>TestHtml</b>", "<b>HtmlValue</b>", new object[] { 3 }, false, BuildExpectedSearchedPath()));
            _mockedHtmlLocalizer["<b>FormattedHtml {0}</b>", 3].Returns(new LocalizedHtmlContent("<b>FormattedHtml {0}</b>", "<b>FormattedValue {0}</b>", new object[] { 3 }, false, BuildExpectedSearchedPath()));
        }

        [Fact]
        public void ViewLocalizer_Constructor_WithNullLocalizer_ShouldThrow()
        {
            Assert.Throws<ArgumentNullException>(() => new ViewLocalizer(null));
        }

        [Theory]
        [InlineData("NotFound", "NotFound", true)]
        [InlineData("TestString", "StringValue", false)]
        [InlineData("<b>TestHtml</b>", "<b>HtmlValue</b>", false)]
        public void ViewLocalizer_This_ShouldPass(string resourceName, string expectedValue, bool expectedNotToBeFound)
        {
            var localizer = CreateLocalizer();
            var subject = localizer[resourceName];
            var _ = _mockedHtmlLocalizer.Received()[resourceName];
            AssertLocalizedHtmlContentResult(subject, resourceName, expectedValue, expectedNotToBeFound);
        }

        [Fact]
        public void ViewLocalizer_This_WithArguments_ShouldPass()
        {
            var localizer = CreateLocalizer();
            var subject = localizer["<b>FormattedHtml {0}</b>", 3];
            var _ = _mockedHtmlLocalizer.Received()["<b>FormattedHtml {0}</b>"];
            AssertLocalizedHtmlContentResult(subject, "<b>FormattedHtml {0}</b>", "<b>FormattedValue {0}</b>", false);
        }

        [Fact]
        public void ViewLocalizer_Contextualize_WithNullViewContext_ShouldThrow()
        {
            var localizer = CreateLocalizer();
            Assert.Throws<ArgumentNullException>(() => localizer.Contextualize(null));
        }

        [Fact]
        public void ViewLocalizer_Contextualize_WithExecutingFilePath_ShouldPass()
        {
            var localizer = CreateLocalizer();
            var viewContext = new ViewContext
            {
                ExecutingFilePath = "/Controller/Action"
            };
            
            localizer.Contextualize(viewContext);
            
            _mockedHtmlLocalizer.Received().SetResourcesSource("Controller.Action");
        }

        [Fact]
        public void ViewLocalizer_Contextualize_ViewPath_ShouldPass()
        {
            var localizer = CreateLocalizer();
            var mockedView = Substitute.For<IView>();
            mockedView.Path.Returns("/Controller/Action");
            var viewContext = new ViewContext { View = mockedView, };

            localizer.Contextualize(viewContext);
            
            _mockedHtmlLocalizer.Received().SetResourcesSource("Controller.Action");
        }

        private ViewLocalizer CreateLocalizer()
        {
            return new ViewLocalizer(_mockedHtmlLocalizer);
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

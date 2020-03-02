using System;
using System.Globalization;
using Microsoft.Extensions.Logging.Abstractions;
using TrdP.Localization.Abstractions;
using TrdP.UnitTestsResources;
using Xunit;

namespace TrdP.Localization.Tests
{
    public class StringLocalizerTests
    {
        private const string SOURCE_ASSEMBLY_NAME = "TrdP.UnitTestsResources";

        [Fact]
        public void StringLocalizer_Constructor_WithLogger_ShouldPass()
        {
            var source = typeof(TestResources);
            var _ = new StringLocalizer(source.Assembly, null, "Some.Path", NullLogger.Instance);
        }

        [Fact]
        public void StringLocalizer_This_WithNullName_ShouldThrow()
        {
            var localizer = CreateLocalizer();
            Assert.Throws<ArgumentNullException>(() => localizer[null]);
        }

        [Theory]
        [InlineData(null, "TestString", "TestString", true)]
        [InlineData(null, "NotFound", "NotFound", true)]
        [InlineData("pt-BR", "TestString", "TestValue", false)]
        [InlineData("pt-BR", "NotFound", "NotFound", true)]
        [InlineData("fr", "TestString", "TestString", true)]
        [InlineData("fr", "NotFound", "NotFound", true)]
        public void StringLocalizer_This_ShouldPass(string cultureName, string resourceName, string expectedValue, bool expectedNotToBeFound)
        {
            var localizer = CreateLocalizer();
            SetCurrentUiCulture(cultureName);
            var result = localizer[resourceName];
            AssertLocalizedStringResult(result, resourceName, expectedValue, expectedNotToBeFound, "TestResources", cultureName);
        }

        [Theory]
        [InlineData(null, "TestString", "TestString", true)]
        [InlineData(null, "MovedString", "MovedString", true)]
        [InlineData(null, "NotFound", "NotFound", true)]
        [InlineData("pt-BR", "TestString", "TestString", true)]
        [InlineData("pt-BR", "MovedString", "MovedValue", false)]
        public void StringLocalizer_This_FromFolder_ShouldPass(string cultureName, string resourceName, string expectedValue, bool expectedNotToBeFound)
        {
            var localizer = CreateLocalizer("Resources", "MovedResources");
            SetCurrentUiCulture(cultureName);
            var result = localizer[resourceName];
            AssertLocalizedStringResult(result, resourceName, expectedValue, expectedNotToBeFound, "Resources.MovedResources", cultureName);
        }

        [Fact]
        public void StringLocalizer_This_ForNotFound_UsingCache_ShouldPass()
        {
            var localizer = CreateLocalizer();
            var result1 = localizer["NotFound"];
            var result2 = localizer["NotFound"];
            AssertLocalizedStringResult(result1, "NotFound", "NotFound", true, "TestResources", "en-US");
            AssertLocalizedStringResult(result2, "NotFound", "NotFound", true, "TestResources", "en-US");
        }

        [Theory]
        [InlineData(null, "TestString", "TestString", true)]
        [InlineData(null, "OtherString", "OtherString", true)]
        [InlineData(null, "NotFound", "NotFound", true)]
        [InlineData("pt-BR", "TestString", "TestString", true)]
        [InlineData("pt-BR", "OtherString", "OtherValue", false)]
        public void StringLocalizer_This_FromInternalFolder_ShouldPass(string cultureName, string resourceName, string expectedValue, bool expectedNotToBeFound)
        {
            var localizer = CreateLocalizer(targetRelativePath: "Internal.OtherResources");
            SetCurrentUiCulture(cultureName);
            var result = localizer[resourceName];
            AssertLocalizedStringResult(result, resourceName, expectedValue, expectedNotToBeFound, "Internal.OtherResources", cultureName);
        }

        [Theory]
        [InlineData(null, "FormattedString {0}", "FormattedString 3", true)]
        [InlineData(null, "NotFound {0}", "NotFound 3", true)]
        [InlineData("pt-BR", "FormattedString {0}", "FormattedValue 3", false)]
        [InlineData("pt-BR", "NotFound {0}", "NotFound 3", true)]
        [InlineData("fr", "FormattedString {0}", "FormattedString 3", true)]
        [InlineData("fr", "NotFound {0}", "NotFound 3", true)]
        public void StringLocalizer_This_WithParams_ShouldPass(string cultureName, string resourceName, string expectedValue, bool expectedNotToBeFound)
        {
            var localizer = CreateLocalizer();
            SetCurrentUiCulture(cultureName);
            var result = localizer[resourceName, 3];
            AssertLocalizedStringResult(result, resourceName, expectedValue, expectedNotToBeFound, "TestResources", cultureName);
        }

        [Fact]
        public void StringLocalizer_This_WithInvalidFormat_ShouldThrow()
        {
            var localizer = CreateLocalizer();
            Assert.Throws<FormatException>(() => localizer["InvalidFormat {0} {1}", 3]);
        }

        [Fact]
        public void StringLocalizer_This_ForSwappingCultures_ShouldPass()
        {
            var localizer = CreateLocalizer();
            CultureInfo.CurrentUICulture = new CultureInfo("pt-BR");
            var result1 = localizer["TestString"];
            CultureInfo.CurrentUICulture = new CultureInfo("fr");
            var result2 = localizer["TestString"];

            AssertLocalizedStringResult(result1, "TestString", "TestValue", false, "TestResources", "pt-BR");
            AssertLocalizedStringResult(result2, "TestString", "TestString", true, "TestResources", "fr");
        }

        private static void SetCurrentUiCulture(string cultureName)
        {
            if (cultureName != null)
            {
                CultureInfo.CurrentUICulture = new CultureInfo(cultureName);
            }
        }

        private StringLocalizer CreateLocalizer(string resourcesRoot = null, string targetRelativePath = null)
        {
            var source = typeof(TestResources);
            return new StringLocalizer(source.Assembly, resourcesRoot, targetRelativePath ?? "TestResources");
        }

        // ReSharper disable ParameterOnlyUsedForPreconditionCheck.Local
        private void AssertLocalizedStringResult(LocalizedString result, string resourceName, string expectedValue, bool expectedNotToBeFound, string baseName, string cultureName)
        {
            Assert.Equal(expectedValue, result);
            Assert.Equal(resourceName, result.Name);
            Assert.Equal(expectedValue, result.Value);
            Assert.Equal(expectedValue, result.ToString());
            Assert.Equal(expectedNotToBeFound, result.ResourceWasNotFound);
            Assert.Equal(BuildExpectedSearchedPath(baseName, cultureName), result.SearchedLocation);
        }
        // ReSharper restore ParameterOnlyUsedForPreconditionCheck.Local

        private static string BuildExpectedSearchedPath(string resourcesPath, string cultureName)
        {
            var finalCultureName = cultureName ?? CultureInfo.CurrentUICulture.Name;
            return $"{SOURCE_ASSEMBLY_NAME}.{resourcesPath}.{finalCultureName}.resx";
        }
    }
}

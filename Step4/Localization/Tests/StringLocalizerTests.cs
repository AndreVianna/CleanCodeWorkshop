using System;
using System.Globalization;
using System.Reflection;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using TrdP.Localization.Abstractions;
using TrdP.Localization.TestData;
using Xunit;

namespace TrdP.Localization.Tests
{
    public class StringLocalizerTests
    {
        private const string SOURCE_ASSEMBLY_NAME = "TrdP.Localization.TestData";
        private readonly Assembly _sourceAssembly;

        public StringLocalizerTests()
        {
            _sourceAssembly = Assembly.Load(SOURCE_ASSEMBLY_NAME);
        }
        
        [Fact]
        public void StringLocalizer_Constructor_WithNullAssembly_ShouldThrow()
        {
            Assert.Throws<ArgumentNullException>(() => new StringLocalizer(null, null));
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public void StringLocalizer_Constructor_WithInvalidResourceFileRelativePath_ShouldThrow(string invalidValue)
        {
            Assert.Throws<ArgumentException>(() => new StringLocalizer(_sourceAssembly, invalidValue));
        }

        [Fact]
        public void StringLocalizer_This_WithNullName_ShouldThrow()
        {
            var localizer = CreateLocalizer("TestResources");
            Assert.Throws<ArgumentNullException>(() => localizer[null]);
        }

        [Theory]
        [InlineData(null, "TestString", "TestString", true)]
        [InlineData(null, "NotFound", "NotFound", true)]
        [InlineData("pt-BR", "TestString", "TestValue", false)]
        [InlineData("pt-BR", "NotFound", "NotFound", true)]
        [InlineData("fr", "TestString", "TestString", true)]
        [InlineData("fr", "NotFound", "NotFound", true)]
        public void StringLocalizer_This_FromRoot_ShouldPass(string cultureName, string resourceName, string expectedValue, bool expectedNotToBeFound)
        {
            var localizer = CreateLocalizer("TestResources");
            SetCurrentUiCulture(cultureName);
            var result = localizer[resourceName];
            AssertLocalizedStringResult(result, resourceName, expectedValue, expectedNotToBeFound, "TestResources", cultureName);
        }

        [Fact]
        public void StringLocalizer_This_ForNotFound_UsingCache_ShouldPass()
        {
            var localizer = CreateLocalizer("TestResources");
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
        [InlineData("pt-BR", "NotFound", "NotFound", true)]
        [InlineData("fr", "TestString", "TestString", true)]
        [InlineData("fr", "OtherString", "OtherString", true)]
        [InlineData("fr", "NotFound", "NotFound", true)]
        public void StringLocalizer_This_FromInternalFolder_ShouldPass(string cultureName, string resourceName, string expectedValue, bool expectedNotToBeFound)
        {
            var localizer = CreateLocalizer("Internal.OtherResources");
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
            var localizer = CreateLocalizer("TestResources");
            SetCurrentUiCulture(cultureName);
            var result = localizer[resourceName, 3];
            AssertLocalizedStringResult(result, resourceName, expectedValue, expectedNotToBeFound, "TestResources", cultureName);
        }

        [Fact]
        public void StringLocalizer_This_FromRoot_WithInvalidFormat_ShouldThrow()
        {
            var localizer = CreateLocalizer("TestResources");
            Assert.Throws<FormatException>(() => localizer["InvalidFormat {0} {1}", 3]);
        }

        [Fact]
        public void StringLocalizer_This_ForSwappingCultures_ShouldPass()
        {
            var localizer = CreateLocalizer("TestResources");
            CultureInfo.CurrentUICulture = new CultureInfo("pt-BR");
            var result1 = localizer["TestString"];
            CultureInfo.CurrentUICulture = new CultureInfo("fr");
            var result2 = localizer["TestString"];

            AssertLocalizedStringResult(result1, "TestString", "TestValue", false, "TestResources", "pt-BR");
            AssertLocalizedStringResult(result2, "TestString", "TestString", true, "TestResources", "fr");
        }

        [Fact]
        public void StringLocalizerOfT_Constructor_WithNullFactory_ShouldThrow()
        {
            Assert.Throws<ArgumentNullException>(() => new StringLocalizer<TestResources>(null));
        }

        [Fact]
        public void StringLocalizerOfT_This_ShouldPass()
        {
            var localizer = CreateLocalizer<TestResources>();
            CultureInfo.CurrentUICulture = new CultureInfo("pt-BR");
            var subject = localizer["TestString"];
            Assert.Equal("TestValue", subject);
        }

        [Fact]
        public void StringLocalizerOfT_This_WithParams_ShouldPass()
        {
            var localizer = CreateLocalizer<TestResources>();
            CultureInfo.CurrentUICulture = new CultureInfo("pt-BR");
            var subject = localizer["FormattedString {0}", 3];
            Assert.Equal("FormattedValue 3", subject);
        }

        private StringLocalizer CreateLocalizer(string resourcesPath)
        {
            return new StringLocalizer(_sourceAssembly, resourcesPath);
        }

        private StringLocalizer<TResource> CreateLocalizer<TResource>() where TResource : class
        {
            var emptyOptions = new OptionsWrapper<LocalizerOptions>(new LocalizerOptions());
            var factory = new StringLocalizerFactory(emptyOptions, NullLoggerFactory.Instance);
            return new StringLocalizer<TResource>(factory);
        }

        private static void SetCurrentUiCulture(string cultureName)
        {
            if (cultureName != null)
            {
                CultureInfo.CurrentUICulture = new CultureInfo(cultureName);
            }
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

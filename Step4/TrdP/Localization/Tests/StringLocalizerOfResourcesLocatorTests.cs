using System;
using System.Globalization;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using TrdP.Localization.Abstractions;
using TrdP.UnitTestsResources;
using TrdP.UnitTestsResources.Internal;
using Xunit;

namespace TrdP.Localization.Tests
{
    public class StringLocalizerOfResourcesLocatorTests
    {
        private const string SOURCE_ASSEMBLY_NAME = "TrdP.UnitTestsResources";

        [Fact]
        public void StringLocalizerOfResourcesLocator_Constructor_WithNullFactory_ShouldThrow()
        {
            Assert.Throws<ArgumentNullException>(() => new StringLocalizer<TestResources>(null));
        }

        [Theory]
        [InlineData(null, "TestString", "TestString", true)]
        [InlineData(null, "NotFound", "NotFound", true)]
        [InlineData("pt-BR", "TestString", "TestValue", false)]
        [InlineData("pt-BR", "NotFound", "NotFound", true)]
        [InlineData("fr", "TestString", "TestString", true)]
        [InlineData("fr", "NotFound", "NotFound", true)]
        public void StringLocalizerOfResourcesLocator_This_ShouldPass(string cultureName, string resourceName, string expectedValue, bool expectedNotToBeFound)
        {
            var localizer = CreateLocalizer<TestResources>();
            SetCurrentUiCulture(cultureName);
            var result = localizer[resourceName];
            AssertLocalizedStringResult(result, resourceName, expectedValue, expectedNotToBeFound, "TestResources", cultureName);
        }

        [Theory]
        [InlineData(null, "TestString", "TestString", true)]
        [InlineData(null, "OtherString", "OtherString", true)]
        [InlineData(null, "NotFound", "NotFound", true)]
        [InlineData("pt-BR", "TestString", "TestString", true)]
        [InlineData("pt-BR", "OtherString", "OtherValue", false)]
        public void StringLocalizerOfResourcesLocator_This_FromInternalFolder_ShouldPass(string cultureName, string resourceName, string expectedValue, bool expectedNotToBeFound)
        {
            var localizer = CreateLocalizer<OtherResources>();
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
        public void StringLocalizerOfResourcesLocator_This_WithParams_ShouldPass(string cultureName, string resourceName, string expectedValue, bool expectedNotToBeFound)
        {
            var localizer = CreateLocalizer<TestResources>();
            SetCurrentUiCulture(cultureName);
            var result = localizer[resourceName, 3];
            AssertLocalizedStringResult(result, resourceName, expectedValue, expectedNotToBeFound, "TestResources", cultureName);
        }

        private static void SetCurrentUiCulture(string cultureName)
        {
            if (cultureName != null)
            {
                CultureInfo.CurrentUICulture = new CultureInfo(cultureName);
            }
        }

        private StringLocalizer<TResourcesLocator> CreateLocalizer<TResourcesLocator>(string resourcesRoot = null)
            where TResourcesLocator : class
        {
            var options = new OptionsWrapper<LocalizationProviderOptions>(new LocalizationProviderOptions
            {
                ResourcesRoot = resourcesRoot
            });
            var factory = new StringLocalizerFactory<TestResources>(options, NullLoggerFactory.Instance);
            return new StringLocalizer<TResourcesLocator>(factory);
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
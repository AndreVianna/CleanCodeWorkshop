using System;
using System.Globalization;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using TrdP.Localization.Abstractions;
using TrdP.Localization.TestData;
using TrdP.Localization.TestData.Internal;
using Xunit;

namespace TrdP.Localization.Tests
{
    public class StringLocalizerOfTTests
    {
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

        [Fact]
        public void StringLocalizerOfT_SetResourcesSource_ShouldPass()
        {
            var localizer = CreateLocalizer<TestResources>();
            SetCurrentUiCulture("pt-Br");
            var result1Before = localizer["TestString"];
            var result2Before = localizer["OtherString"];
            localizer.SetResourcesSource<OtherResources>();
            var result1After = localizer["TestString"];
            var result2After = localizer["OtherString"];

            AssertLocalizedStringResult<TestResources>(result1Before, "TestString", "TestValue", false);
            AssertLocalizedStringResult<TestResources>(result2Before, "OtherString", "OtherString", true);
            AssertLocalizedStringResult<OtherResources>(result1After, "TestString", "TestString", true);
            AssertLocalizedStringResult<OtherResources>(result2After, "OtherString", "OtherValue", false);
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
        private void AssertLocalizedStringResult<T>(LocalizedString result, string resourceName, string expectedValue, bool expectedNotToBeFound)
        {
            Assert.Equal(expectedValue, result);
            Assert.Equal(resourceName, result.Name);
            Assert.Equal(expectedValue, result.Value);
            Assert.Equal(expectedValue, result.ToString());
            Assert.Equal(expectedNotToBeFound, result.ResourceWasNotFound);
            Assert.Equal(BuildExpectedSearchedPath<T>(), result.SearchedLocation);
        }
        // ReSharper restore ParameterOnlyUsedForPreconditionCheck.Local

        private static string BuildExpectedSearchedPath<T>()
        {
            var resourcesPath = typeof(T).FullName;
            return $"{resourcesPath}.pt-BR.resx";
        }
    }
}

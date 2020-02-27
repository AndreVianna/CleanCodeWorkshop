using System;
using System.Globalization;
using Microsoft.Extensions.Options;
using TrdP.Localization.TestData;
using Xunit;

namespace TrdP.Localization.Tests
{
    public class StringLocalizerTests
    {
        [Fact]
        public void StringLocalizer_Constructor_WithNullFactory_ShouldThrow()
        {
            Assert.Throws<ArgumentNullException>(() => new StringLocalizer<TestResources>(null));
        }

        [Fact]
        public void ResourceManagerStringLocalizer_This_ShouldPass()
        {
            var localizer = CreateLocalizer<TestResources>();
            CultureInfo.CurrentUICulture = new CultureInfo("pt-BR");
            var subject = localizer["TestString"];
            Assert.Equal("TestValue", subject);
        }

        [Fact]
        public void ResourceManagerStringLocalizer_This_WithParams_ShouldPass()
        {
            var localizer = CreateLocalizer<TestResources>();
            CultureInfo.CurrentUICulture = new CultureInfo("pt-BR");
            var subject = localizer["FormattedString {0}", 3];
            Assert.Equal("FormattedValue 3", subject);
        }

        [Fact]
        public void ResourceManagerStringLocalizer_This_WithCulture_ShouldPass()
        {
            var localizer = CreateLocalizer<TestResources>();
            var subject = localizer.WithCulture(new CultureInfo("en-US"));
            Assert.NotNull(subject);
        }

        private StringLocalizer<T> CreateLocalizer<T>()
        {
            return new StringLocalizer<T>(new ResourceManagerStringLocalizerFactory(new OptionsWrapper<LocalizerOptions>(new LocalizerOptions())));
        }
    }
}
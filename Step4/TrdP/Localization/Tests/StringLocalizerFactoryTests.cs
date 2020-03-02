using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using TrdP.UnitTestsResources;
using Xunit;

namespace TrdP.Localization.Tests
{
    public class StringLocalizerFactoryTests
    {
        private static readonly IOptions<LocalizerProviderOptions> _optionsValue = new OptionsWrapper<LocalizerProviderOptions>(new LocalizerProviderOptions());

        [Fact]
        public void StringLocalizerFactory_Constructor_WithNullOptions_ShouldThrow()
        {
            Assert.Throws<ArgumentNullException>(() => new StringLocalizerFactory<TestResources>(null, null));
        }

        [Fact]
        public void StringLocalizerFactory_Constructor_WithNullLoggerFactory_ShouldThrow()
        {
            Assert.Throws<ArgumentNullException>(() => new StringLocalizerFactory<TestResources>(_optionsValue, null));
        }

        [Theory]
        [InlineData(null, "")]
        [InlineData("", "")]
        [InlineData(" ", "")]
        [InlineData("Resources", "Resources")]
        [InlineData("Internal/Resources", "Internal.Resources")]
        [InlineData("Internal\\Resources", "Internal.Resources")]
        [InlineData("/Internal/Resources ", "Internal.Resources")]
        [InlineData("Internal.Resources", "Internal.Resources")]
        public void StringLocalizerFactory_Constructor_WithResourceRoot_ShouldPass(string resourcesRoot, string expectedResourcesRoot)
        {
            _optionsValue.Value.ResourcesRoot = resourcesRoot;
            var factory = new StringLocalizerFactory<TestResources>(_optionsValue, NullLoggerFactory.Instance);
            Assert.Equal(typeof(TestResources).Assembly.GetName().Name, factory.ProviderAssembly.GetName().Name);
            Assert.Equal(expectedResourcesRoot, factory.ResourcesRoot);
        }

        [Fact]
        public void StringLocalizerFactory_Constructor_WithAvailableCultures_ShouldPass()
        {
            _optionsValue.Value.AvailableCultures = new List<CultureInfo>
            {
                new CultureInfo("en-US"),
                new CultureInfo("pt-BR"),
            };
            var factory = new StringLocalizerFactory<TestResources>(_optionsValue, NullLoggerFactory.Instance);
            Assert.Equal(typeof(TestResources).Assembly.GetName().Name, factory.ProviderAssembly.GetName().Name);
            Assert.Equal(2, factory.AvailableCultures.Count());
            Assert.Equal("en-US", factory.AvailableCultures.ElementAt(0).Name);
            Assert.Equal("pt-BR", factory.AvailableCultures.ElementAt(1).Name);
        }

        [Fact]
        public void StringLocalizerFactory_Constructor_WithNullAvailableCultures_ShouldPass()
        {
            _optionsValue.Value.AvailableCultures = null;
            var factory = new StringLocalizerFactory<TestResources>(_optionsValue, NullLoggerFactory.Instance);
            Assert.Equal(typeof(TestResources).Assembly.GetName().Name, factory.ProviderAssembly.GetName().Name);
            Assert.Empty(factory.AvailableCultures);
        }

        [Fact]
        public void StringLocalizerFactory_Create_ForType_ShouldPass()
        {
            var factory = new StringLocalizerFactory<TestResources>(_optionsValue, NullLoggerFactory.Instance);
            var result = factory.Create<StringLocalizerFactoryTests>();
            Assert.NotNull(result);
        }

        [Fact]
        public void StringLocalizerFactory_Create_ForPath_ShouldPass()
        {
            var factory = new StringLocalizerFactory<TestResources>(_optionsValue, NullLoggerFactory.Instance);
            var result = factory.Create("Some.Path");
            Assert.NotNull(result);
        }
    }
}
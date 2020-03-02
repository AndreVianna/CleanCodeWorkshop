using System;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using TrdP.UnitTestResources;
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
        public void StringLocalizerFactory_Create_ShouldPass()
        {
            var factory = new StringLocalizerFactory<TestResources>(_optionsValue, NullLoggerFactory.Instance);
            var result = factory.Create<StringLocalizerFactoryTests>();
            Assert.NotNull(result);
        }
    }
}
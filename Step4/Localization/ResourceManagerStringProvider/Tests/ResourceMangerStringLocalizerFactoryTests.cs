using System;
using System.Globalization;
using Microsoft.Extensions.Options;
using TrdP.ResourceManagerStringProvider.Localization;
using TrdP.ResourceManagerStringProvider.Localization.Internal;
using Xunit;

namespace TrdP.ResourceManagerStringProvider.Tests
{
    public class ResourceMangerStringLocalizerFactoryTests
    {
        private const string SOURCE_ASSEMBLY_NAME = "TrdP.ResourceManagerStringProvider.Localization";

        [Fact]
        public void ResourceMangerStringLocalizerFactory_Constructor_ShouldPass()
        {
            var _ = new ResourceManagerStringLocalizerFactory(new OptionsWrapper<LocalizerOptions>(new LocalizerOptions()));
        }

        [Theory]
        [InlineData("pt-BR", "TestValue")]
        [InlineData("fr", "TestString")]
        public void ResourceMangerStringLocalizerFactory_Create_WithType_ShouldPass(string currentUiCultureName, string expectedValue)
        {
            CultureInfo.CurrentUICulture = new CultureInfo(currentUiCultureName);
            var factory = new ResourceManagerStringLocalizerFactory(new OptionsWrapper<LocalizerOptions>(new LocalizerOptions()));
            var result = factory.Create(typeof(TestResources));
            Assert.NotNull(result);
            Assert.Equal(expectedValue, result["TestString"]);
        }

        [Theory]
        [InlineData("pt-BR", "OtherValue")]
        [InlineData("fr", "OtherString")]
        public void ResourceMangerStringLocalizerFactory_Create_WithType_ForInternalType_ShouldPass(string currentUiCultureName, string expectedValue)
        {
            CultureInfo.CurrentUICulture = new CultureInfo(currentUiCultureName);
            var factory = new ResourceManagerStringLocalizerFactory(new OptionsWrapper<LocalizerOptions>(new LocalizerOptions()));
            var result = factory.Create(typeof(OtherResources));
            Assert.NotNull(result);
            Assert.Equal(expectedValue, result["OtherString"]);
        }

        [Theory]
        [InlineData("pt-BR", "TestValue")]
        [InlineData("fr", "TestString")]
        public void ResourceMangerStringLocalizerFactory_Create_WithSourceAndAssembly_ShouldPass(string currentUiCultureName, string expectedValue)
        {
            CultureInfo.CurrentUICulture = new CultureInfo(currentUiCultureName);
            var factory = new ResourceManagerStringLocalizerFactory(new OptionsWrapper<LocalizerOptions>(new LocalizerOptions()));
            var result = factory.Create("TestResources", SOURCE_ASSEMBLY_NAME);
            Assert.NotNull(result);
            Assert.Equal(expectedValue, result["TestString"]);
        }

        [Theory]
        [InlineData("pt-BR", "OtherValue")]
        [InlineData("fr", "OtherString")]
        public void ResourceMangerStringLocalizerFactory_Create_WithSourceAndAssembly_ForInternalResource_ShouldPass(string currentUiCultureName, string expectedValue)
        {
            CultureInfo.CurrentUICulture = new CultureInfo(currentUiCultureName);
            var factory = new ResourceManagerStringLocalizerFactory(new OptionsWrapper<LocalizerOptions>(new LocalizerOptions()));
            var result = factory.Create("Internal/OtherResources", SOURCE_ASSEMBLY_NAME);
            Assert.NotNull(result);
            Assert.Equal(expectedValue, result["OtherString"]);
        }

        [Theory]
        [InlineData("pt-BR", "MovedValue")]
        [InlineData("fr", "MovedString")]
        public void ResourceMangerStringLocalizerFactory_Create_WithOptions_ShouldPass(string currentUiCultureName, string expectedValue)
        {
            CultureInfo.CurrentUICulture = new CultureInfo(currentUiCultureName);
            var factory = new ResourceManagerStringLocalizerFactory(new OptionsWrapper<LocalizerOptions>(new LocalizerOptions
            {
                ResourcesRoot = "Resources",
            }));
            var result = factory.Create(typeof(MovedResources));
            Assert.NotNull(result);
            Assert.Equal(expectedValue, result["MovedString"]);
        }

        [Theory]
        [InlineData("pt-BR", "MovedString")]
        [InlineData("fr", "MovedString")]
        public void ResourceMangerStringLocalizerFactory_Create_WithNullResourceRoot_ShouldPass(string currentUiCultureName, string expectedValue)
        {
            CultureInfo.CurrentUICulture = new CultureInfo(currentUiCultureName);
            var factory = new ResourceManagerStringLocalizerFactory(new OptionsWrapper<LocalizerOptions>(new LocalizerOptions
            {
                ResourcesRoot = null,
            }));
            var result = factory.Create(typeof(MovedResources));
            Assert.NotNull(result);
            Assert.Equal(expectedValue, result["MovedString"]);
        }

        [Fact]
        public void ResourceMangerStringLocalizerFactory_Constructor_WithNullOptions_ShouldThrow()
        {
            Assert.Throws<ArgumentNullException>(() => new ResourceManagerStringLocalizerFactory(null));
        }


        [Fact]
        public void ResourceMangerStringLocalizerFactory_Create_WithNullSource_ShouldThrow()
        {
            var factory = new ResourceManagerStringLocalizerFactory(new OptionsWrapper<LocalizerOptions>(new LocalizerOptions()));
            Assert.Throws<ArgumentNullException>(() => factory.Create(null));
        }


        [Theory]
        [InlineData(null, null)]
        [InlineData("", null)]
        [InlineData(" ", null)]
        [InlineData("valid", null)]
        [InlineData("valid", "")]
        [InlineData("valid", " ")]
        public void ResourceMangerStringLocalizerFactory_Create_WithInvalidNames_ShouldThrow(string sourceName, string assemblyName)
        {
            var factory = new ResourceManagerStringLocalizerFactory(new OptionsWrapper<LocalizerOptions>(new LocalizerOptions()));
            Assert.Throws<ArgumentException>(() => factory.Create(sourceName, assemblyName));
        }
    }
}

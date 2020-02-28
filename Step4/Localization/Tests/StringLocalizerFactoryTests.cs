using System;
using System.Globalization;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using TrdP.Localization.TestData;
using TrdP.Localization.TestData.Internal;
using Xunit;

namespace TrdP.Localization.Tests
{
    public class StringLocalizerFactoryTests
    {
        private const string SOURCE_ASSEMBLY_NAME = "TrdP.Localization.TestData";
        private static readonly IOptions<LocalizerOptions> _emptyOptions = new OptionsWrapper<LocalizerOptions>(new LocalizerOptions());

        [Fact]
        public void StringLocalizerFactory_Constructor_ShouldPass()
        {
            var _ = new StringLocalizerFactory(_emptyOptions, NullLoggerFactory.Instance);
        }

        [Fact]
        public void StringLocalizerFactory_Constructor_WithNullOptions_ShouldThrow()
        {
            Assert.Throws<ArgumentNullException>(() => new StringLocalizerFactory(null, null));
        }


        [Fact]
        public void StringLocalizerFactory_Constructor_WithNullLoggerFactory_ShouldThrow()
        {
            Assert.Throws<ArgumentNullException>(() => new StringLocalizerFactory(_emptyOptions, null));
        }

        [Theory]
        [InlineData("pt-BR", "TestValue")]
        [InlineData("fr", "TestString")]
        public void StringLocalizerFactory_Create_WithType_ShouldPass(string currentUiCultureName, string expectedValue)
        {
            CultureInfo.CurrentUICulture = new CultureInfo(currentUiCultureName);
            var factory = new StringLocalizerFactory(_emptyOptions, NullLoggerFactory.Instance);
            var result = factory.Create(typeof(TestResources));
            Assert.NotNull(result);
            Assert.Equal(expectedValue, result["TestString"]);
        }

        [Theory]
        [InlineData("pt-BR", "OtherValue")]
        [InlineData("fr", "OtherString")]
        public void StringLocalizerFactory_Create_WithType_ForInternalType_ShouldPass(string currentUiCultureName, string expectedValue)
        {
            CultureInfo.CurrentUICulture = new CultureInfo(currentUiCultureName);
            var factory = new StringLocalizerFactory(_emptyOptions, NullLoggerFactory.Instance);
            var result = factory.Create(typeof(OtherResources));
            Assert.NotNull(result);
            Assert.Equal(expectedValue, result["OtherString"]);
        }

        [Theory]
        [InlineData("pt-BR", "TestValue")]
        [InlineData("fr", "TestString")]
        public void StringLocalizerFactory_Create_WithSourceAndAssembly_ShouldPass(string currentUiCultureName, string expectedValue)
        {
            CultureInfo.CurrentUICulture = new CultureInfo(currentUiCultureName);
            var factory = new StringLocalizerFactory(_emptyOptions, NullLoggerFactory.Instance);
            var result = factory.Create("TestResources", SOURCE_ASSEMBLY_NAME);
            Assert.NotNull(result);
            Assert.Equal(expectedValue, result["TestString"]);
        }

        [Theory]
        [InlineData("pt-BR", "OtherValue")]
        [InlineData("fr", "OtherString")]
        public void StringLocalizerFactory_Create_WithSourceAndAssembly_ForInternalResource_ShouldPass(string currentUiCultureName, string expectedValue)
        {
            CultureInfo.CurrentUICulture = new CultureInfo(currentUiCultureName);
            var factory = new StringLocalizerFactory(_emptyOptions, NullLoggerFactory.Instance);
            var result = factory.Create("Internal/OtherResources", SOURCE_ASSEMBLY_NAME);
            Assert.NotNull(result);
            Assert.Equal(expectedValue, result["OtherString"]);
        }

        [Theory]
        [InlineData("pt-BR", "MovedValue")]
        [InlineData("fr", "MovedString")]
        public void StringLocalizerFactory_Create_WithOptions_ShouldPass(string currentUiCultureName, string expectedValue)
        {
            CultureInfo.CurrentUICulture = new CultureInfo(currentUiCultureName);
            var options = new OptionsWrapper<LocalizerOptions>(new LocalizerOptions
            {
                ResourcesRoot = "Resources",
            });
            var factory = new StringLocalizerFactory(options, NullLoggerFactory.Instance);
            var result = factory.Create(typeof(MovedResources));
            Assert.NotNull(result);
            Assert.Equal(expectedValue, result["MovedString"]);
        }

        [Theory]
        [InlineData("pt-BR", "MovedString")]
        [InlineData("fr", "MovedString")]
        public void StringLocalizerFactory_Create_WithNullResourceRoot_ShouldPass(string currentUiCultureName, string expectedValue)
        {
            CultureInfo.CurrentUICulture = new CultureInfo(currentUiCultureName);
            var options = new OptionsWrapper<LocalizerOptions>(new LocalizerOptions
            {
                ResourcesRoot = null,
            });
            var factory = new StringLocalizerFactory(options, NullLoggerFactory.Instance);
            var result = factory.Create(typeof(MovedResources));
            Assert.NotNull(result);
            Assert.Equal(expectedValue, result["MovedString"]);
        }

        [Fact]
        public void StringLocalizerFactory_Create_WithNullSource_ShouldThrow()
        {
            var factory = new StringLocalizerFactory(_emptyOptions, NullLoggerFactory.Instance);
            Assert.Throws<ArgumentNullException>(() => factory.Create(null));
        }


        [Theory]
        [InlineData(null, null)]
        [InlineData("", null)]
        [InlineData(" ", null)]
        [InlineData("valid", null)]
        [InlineData("valid", "")]
        [InlineData("valid", " ")]
        public void StringLocalizerFactory_Create_WithInvalidNames_ShouldThrow(string sourceName, string assemblyName)
        {
            var factory = new StringLocalizerFactory(_emptyOptions, NullLoggerFactory.Instance);
            Assert.Throws<ArgumentException>(() => factory.Create(sourceName, assemblyName));
        }
    }
}

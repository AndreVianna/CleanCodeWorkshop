using System;
using System.Globalization;
using System.Reflection;
using TrdP.Localization.Abstractions;
using Xunit;

namespace TrdP.Localization.Tests
{
    public class ResourceManagerStringLocalizerTests
    {
        private const string SOURCE_ASSEMBLY_NAME = "TrdP.Localization.TestData";
        private readonly Assembly _sourceAssembly;

        public ResourceManagerStringLocalizerTests()
        {
            _sourceAssembly = Assembly.Load(SOURCE_ASSEMBLY_NAME);
        }
        
        [Fact]
        public void ResourceManagerStringLocalizer_Constructor_WithNullAssembly_ShouldThrow()
        {
            Assert.Throws<ArgumentNullException>(() => new ResourceManagerStringLocalizer(null, null));
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public void ResourceManagerStringLocalizer_Constructor_WithInvalidResourceFileRelativePath_ShouldThrow(string resourceFileRelativePath)
        {
            Assert.Throws<ArgumentException>(() => new ResourceManagerStringLocalizer(_sourceAssembly, resourceFileRelativePath));
        }

        [Fact]
        public void ResourceManagerStringLocalizer_This_WithNullName_ShouldThrow()
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
        public void ResourceManagerStringLocalizer_This_FromRoot_ShouldPass(string cultureName, string resourceName, string expectedValue, bool expectedResourceNotFound)
        {
            var localizer = CreateLocalizer("TestResources", cultureName);
            var result = localizer[resourceName];
            AssertLocalizedStringResult(result, resourceName, expectedValue, expectedResourceNotFound, "TestResources", cultureName);
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
        public void ResourceManagerStringLocalizer_This_FromInternalFolder_ShouldPass(string cultureName, string resourceName, string expectedValue, bool expectedResourceNotFound)
        {
            var localizer = CreateLocalizer("Internal.OtherResources", cultureName);
            var result = localizer[resourceName];
            AssertLocalizedStringResult(result, resourceName, expectedValue, expectedResourceNotFound, "Internal.OtherResources", cultureName);
        }

        [Theory]
        [InlineData(null, "FormattedString {0}", "FormattedString 3", true)]
        [InlineData(null, "NotFound {0}", "NotFound 3", true)]
        [InlineData("pt-BR", "FormattedString {0}", "FormattedValue 3", false)]
        [InlineData("pt-BR", "NotFound {0}", "NotFound 3", true)]
        [InlineData("fr", "FormattedString {0}", "FormattedString 3", true)]
        [InlineData("fr", "NotFound {0}", "NotFound 3", true)]
        public void ResourceManagerStringLocalizer_This_WithParams_ShouldPass(string cultureName, string resourceName, string expectedValue, bool expectedResourceNotFound)
        {
            var localizer = CreateLocalizer("TestResources", cultureName);
            var result = localizer[resourceName, 3];
            AssertLocalizedStringResult(result, resourceName, expectedValue, expectedResourceNotFound, "TestResources", cultureName);
        }

        [Fact]
        public void ResourceManagerStringLocalizer_This_FromRoot_WithInvalidFormat_ShouldThrow()
        {
            var localizer = new ResourceManagerStringLocalizer(_sourceAssembly, "TestResources");
            Assert.Throws<FormatException>(() => localizer["InvalidFormat {0} {1}", 3]);
        }

        [Fact]
        public void ResourceManagerStringLocalizer_This_ForSwappingCultures_ShouldPass()
        {
            var localizer = CreateLocalizer("TestResources");
            CultureInfo.CurrentUICulture = new CultureInfo("pt-BR");
            var result1 = localizer["TestString"];
            CultureInfo.CurrentUICulture = new CultureInfo("fr");
            var result2 = localizer["TestString"];

            AssertLocalizedStringResult(result1, "TestString", "TestValue", false, "TestResources", "pt-BR");
            AssertLocalizedStringResult(result2, "TestString", "TestString", true, "TestResources", "fr");
        }

        [Theory]
        [InlineData("pt-BR")]
        [InlineData("fr")]
        public void ResourceManagerStringLocalizer_WithCulture_ShouldThrow(string cultureName)
        {
            var localizer = CreateLocalizer("TestResources");
            var result = localizer.WithCulture(new CultureInfo(cultureName));
            Assert.NotNull(result);
        }

        private ResourceManagerStringLocalizer CreateLocalizer(string resourcesPath, string cultureName = null)
        {
            var culture = cultureName == null
                ? null
                : new CultureInfo(cultureName);
            var localizer = new ResourceManagerStringLocalizer(_sourceAssembly, resourcesPath, culture);
            return localizer;
        }

        // ReSharper disable ParameterOnlyUsedForPreconditionCheck.Local
        private void AssertLocalizedStringResult(LocalizedString result, string resourceName, string expectedValue, 
            bool expectedFound, string baseName, string cultureName)
        {
            Assert.Equal(expectedValue, result);
            Assert.Equal(resourceName, result.Name);
            Assert.Equal(expectedValue, result.Value);
            Assert.Equal(expectedValue, result.ToString());
            Assert.Equal(expectedFound, result.ResourceNotFound);
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

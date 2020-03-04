using System.Collections.Generic;
using System.Globalization;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using TrdP.Common.TestDoubles;
using TrdP.Localization;
using TrdP.UnitTestsResources;
using Xunit;

namespace TrdP.Mvc.Localization.Tests
{
    public class MvcBuilderExtensionsTests
    {
        private static readonly IServiceCollection _fakeServiceCollection = new FakeServiceCollection();
        private static readonly IMvcBuilder _mockedMvcBuilder = new FakeMvcBuilder();

        [Fact]
        public void MvcBuilderExtensions_AddLocalizationProvider_ShouldPass()
        {
            _mockedMvcBuilder.AddLocalizationProvider<TestResources>();

            AssertConfiguration("", false);
        }

        [Fact]
        public void MvcBuilderExtensions_AddLocalizationProvider_WithConfiguration_ShouldPass()
        {
            _mockedMvcBuilder.AddLocalizationProvider<TestResources>(opt =>
            {
                opt.ResourcesRoot = "Resources";
                opt.AvailableCultures = new List<CultureInfo> { new CultureInfo("pt-BR") };
            });

            AssertConfiguration("Resources", true);
        }

        private static void AssertConfiguration(string expectedResourceRoot, bool hasCultures)
        {
            var provider = _fakeServiceCollection.BuildServiceProvider();
            var localizerProviderOptionsConfigurator = provider.GetRequiredService<IConfigureOptions<LocalizationProviderOptions>>();
            var localizerProviderOptions = new LocalizationProviderOptions();
            localizerProviderOptionsConfigurator.Configure(localizerProviderOptions);

            var requestLocalizationConfigurator = provider.GetRequiredService<IConfigureOptions<RequestLocalizationOptions>>();
            var requestLocalizationOptions = new RequestLocalizationOptions();
            requestLocalizationConfigurator.Configure(requestLocalizationOptions);
            Assert.Equal(expectedResourceRoot, localizerProviderOptions.ResourcesRoot);
            if (hasCultures)
            {
                Assert.NotEmpty(localizerProviderOptions.AvailableCultures);
                Assert.Equal("pt-BR", requestLocalizationOptions.DefaultRequestCulture.UICulture.Name);
                Assert.NotEmpty(requestLocalizationOptions.SupportedCultures);
                Assert.NotEmpty(requestLocalizationOptions.SupportedUICultures);
            }
            else
            {
                Assert.Empty(localizerProviderOptions.AvailableCultures);
                Assert.Equal(CultureInfo.CurrentUICulture.Name, requestLocalizationOptions.DefaultRequestCulture.UICulture.Name);
                Assert.Empty(requestLocalizationOptions.SupportedCultures);
                Assert.Empty(requestLocalizationOptions.SupportedUICultures);
            }
        }
    }
}
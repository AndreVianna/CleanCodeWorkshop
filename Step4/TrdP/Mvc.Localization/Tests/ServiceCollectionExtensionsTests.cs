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
    public class ServiceCollectionExtensionsTests
    {
        private static readonly IServiceCollection _fakeServiceCollection = new FakeServiceCollection();

        [Fact]
        public void ServiceCollectionExtensions_AddLocalization_ShouldPass()
        {
            _fakeServiceCollection.AddMvcLocalizationProvider<TestResources>();

            AssertConfiguration("", false);
        }

        [Fact]
        public void ServiceCollectionExtensions_AddLocalization_WithConfiguration_ShouldPass()
        {
            _fakeServiceCollection.AddMvcLocalizationProvider<TestResources>(opt =>
            {
                opt.ResourcesRoot = "Resources";
                opt.AvailableCultures = new List<CultureInfo> { new CultureInfo("pt-BR") };
            });

            AssertConfiguration("Resources", true);
        }

        private static void AssertConfiguration(string expectedResourceRoot, bool hasCultures)
        {
            var provider = _fakeServiceCollection.BuildServiceProvider();
            var localizerProviderOptionsConfigurator = provider.GetRequiredService<IConfigureOptions<LocalizerProviderOptions>>();
            var localizerProviderOptions = new LocalizerProviderOptions();
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
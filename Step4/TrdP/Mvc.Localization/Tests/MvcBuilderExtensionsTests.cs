using System.Globalization;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using TrdP.Common.TestDoubles;
using Xunit;

namespace TrdP.Mvc.Localization.Tests
{
    public class MvcBuilderExtensionsTests
    {
        private static readonly IMvcBuilder _mockedMvcBuilder = new FakeMvcBuilder();

        [Fact]
        public void MvcBuilderExtensions_AddLocalizationProvider_ShouldPass()
        {
            _mockedMvcBuilder.AddLocalization();

            AssertConfiguration("", false);
        }

        [Fact]
        public void MvcBuilderExtensions_AddLocalizationProvider_WithConfiguration_ShouldPass()
        {
            _mockedMvcBuilder.AddLocalization(opt =>
            {
                opt.AddCulture(new CultureInfo("pt-BR"));
            });

            AssertConfiguration("Resources", true);
        }

        [Fact]
        public void MvcBuilderExtensions_AddLocalizationProvider_WithMultipleCultures_ShouldPass()
        {
            _mockedMvcBuilder.AddLocalization(opt =>
            {
                opt.AddCultures(new [] { new CultureInfo("pt-BR"), new CultureInfo("en-US") });
            });

            AssertConfiguration("Resources", true);
        }

        private static void AssertConfiguration(string expectedResourceRoot, bool hasCultures)
        {
            var provider = _mockedMvcBuilder.Services.BuildServiceProvider();
            var localizerProviderOptionsConfigurator = provider.GetRequiredService<IConfigureOptions<MvcLocalizationOptions>>();
            var localizerProviderOptions = new MvcLocalizationOptions();
            localizerProviderOptionsConfigurator.Configure(localizerProviderOptions);

            var requestLocalizationConfigurator = provider.GetRequiredService<IConfigureOptions<RequestLocalizationOptions>>();
            var requestLocalizationOptions = new RequestLocalizationOptions();
            requestLocalizationConfigurator.Configure(requestLocalizationOptions);
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
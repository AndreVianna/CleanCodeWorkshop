using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using TrdP.Common.TestDoubles;
using TrdP.UnitTestsResources;
using Xunit;

namespace TrdP.Localization.Tests
{
    public class ServiceCollectionExtensionsTests
    {
        private static readonly IServiceCollection _fakeServiceCollection = new FakeServiceCollection();

        [Fact]
        public void ServiceCollectionExtensions_AddLocalization_ShouldPass()
        {
            _fakeServiceCollection.AddLocalizationProvider<TestResources>();

            AssertConfiguration("");
        }

        [Fact]
        public void ServiceCollectionExtensions_AddLocalization_WithConfiguration_ShouldPass()
        {
            _fakeServiceCollection.AddLocalizationProvider<TestResources>(opt =>
            {
                opt.ResourcesRoot = "Resources";
            });

            AssertConfiguration("Resources");
        }

        private static void AssertConfiguration(string expectedResourceRoot)
        {
            var provider = _fakeServiceCollection.BuildServiceProvider();
            var configurationService = provider.GetRequiredService<IConfigureOptions<LocalizationProviderOptions>>();
            var options = new LocalizationProviderOptions();
            configurationService.Configure(options);
            Assert.Equal(expectedResourceRoot, options.ResourcesRoot);
        }
    }
}
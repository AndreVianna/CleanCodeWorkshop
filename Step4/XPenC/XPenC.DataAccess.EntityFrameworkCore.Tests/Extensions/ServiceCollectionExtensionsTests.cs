using Microsoft.Extensions.DependencyInjection;
using XPenC.DataAccess.Contracts;
using XPenC.DataAccess.EntityFrameworkCore.Extensions;
using XPenC.DataAccess.EntityFrameworkCore.Tests.TestDoubles;
using Xunit;

namespace XPenC.DataAccess.EntityFrameworkCore.Tests.Extensions
{
    public class ServiceCollectionExtensionsTests
    {
        private readonly FakeConfiguration _configuration;
        private readonly FakeServiceCollection _serviceCollection;

        public ServiceCollectionExtensionsTests()
        {
            _configuration = new FakeConfiguration();
            _serviceCollection = new FakeServiceCollection();
        }

        [Fact]
        public void ServiceCollectionExtensions_AddSqlServerDataContext_ShouldPass()
        {
            _serviceCollection.AddEntityFrameworkDataContext(_configuration, "SomeName");
            var provider = _serviceCollection.BuildServiceProvider();
            Assert.IsType<XPenCDbContext>(provider.GetRequiredService<XPenCDbContext>());
            Assert.IsType<EntityFrameworkDataContext>(provider.GetRequiredService<IDataContext>());
        }
    }
}
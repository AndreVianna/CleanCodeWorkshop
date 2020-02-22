using Microsoft.Extensions.DependencyInjection;
using XPenC.DataAccess.Contracts;
using XPenC.DataAccess.SqlServer.Extensions;
using XPenC.DataAccess.SqlServer.Tests.TestDoubles;
using Xunit;

namespace XPenC.DataAccess.SqlServer.Tests.Extensions
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
            _serviceCollection.AddSqlServerDataContext(_configuration, "SomeName");
            var provider = _serviceCollection.BuildServiceProvider();
            Assert.IsType<SqlDataProvider>(provider.GetRequiredService<ISqlDataProvider>());
            Assert.IsType<SqlServerDataContext>(provider.GetRequiredService<IDataContext>());
        }
    }
}
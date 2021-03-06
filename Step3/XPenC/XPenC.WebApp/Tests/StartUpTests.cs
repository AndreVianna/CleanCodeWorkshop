using Microsoft.Extensions.DependencyInjection;
using XPenC.BusinessLogic;
using XPenC.BusinessLogic.Contracts;
using XPenC.DataAccess.Contracts;
using XPenC.DataAccess.SqlServer;
using XPenC.WebApp.Tests.TestDoubles;
using Xunit;

namespace XPenC.WebApp.Tests
{
    public class StartUpTests
    {
        private readonly FakeServiceCollection _serviceCollection;

        public StartUpTests()
        {
            _serviceCollection = new FakeServiceCollection();
            var startUp = new Startup(new DummyConfiguration());
            startUp.ConfigureServices(_serviceCollection);
        }

        [Fact]
        public void StartUp_ConfigureServices_ShouldPass()
        {
            var provider = _serviceCollection.BuildServiceProvider();
            Assert.IsType<SqlDataProvider>(provider.GetRequiredService<ISqlDataProvider>());
            Assert.IsType<SqlServerDataContext>(provider.GetRequiredService<IDataContext>());
            Assert.IsType<ExpenseReportOperations>(provider.GetRequiredService<IExpenseReportOperations>());
        }
    }
}
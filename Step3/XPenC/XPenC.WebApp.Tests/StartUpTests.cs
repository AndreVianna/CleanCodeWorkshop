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
        private readonly Startup _startUp;

        public StartUpTests()
        {
            _serviceCollection = new FakeServiceCollection();
            _startUp = new Startup(new DummyConfiguration());
            _startUp.ConfigureServices(_serviceCollection);
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
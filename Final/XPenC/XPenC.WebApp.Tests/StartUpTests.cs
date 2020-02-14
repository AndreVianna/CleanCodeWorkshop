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
            Assert.NotNull(_serviceCollection.FindFor<ISqlDataProvider>().ImplementationFactory);
            Assert.Equal(typeof(SqlServerDataContext), _serviceCollection.FindFor<IDataContext>().ImplementationType);
            Assert.Equal(typeof(ExpenseReportOperations), _serviceCollection.FindFor<IExpenseReportOperations>().ImplementationType);
        }

        [Fact(Skip = "Blocked by Route Configuration")]
        public void StartUp_Configure_ForDevelopment_ShouldPass()
        {
            Startup.Configure(new FakeApplicationBuilder(_serviceCollection),  new FakeDevelopmentEnvironment());
        }

        [Fact(Skip = "Blocked by Route Configuration")]
        public void StartUp_Configure_ForNotDevelopment_ShouldPass()
        {
            Startup.Configure(new FakeApplicationBuilder(_serviceCollection), new FakeProductionEnvironment());
        }
    }
}
using Microsoft.Extensions.DependencyInjection;
using XPenC.BusinessLogic;
using XPenC.BusinessLogic.Contracts;
using XPenC.Common.TestDoubles;
using XPenC.DataAccess.Contracts;
using XPenC.DataAccess.EntityFrameworkCore;
using Xunit;

namespace XPenC.WebApp.Tests
{
    public class StartUpTests
    {
        private readonly FakeServiceCollection _serviceCollection;

        public StartUpTests()
        {
            _serviceCollection = new FakeServiceCollection();
            var startUp = new Startup(new FakeConfiguration());
            startUp.ConfigureServices(_serviceCollection);
        }

        [Fact]
        public void StartUp_ConfigureServices_ShouldPass()
        {
            var provider = _serviceCollection.BuildServiceProvider();
            Assert.IsType<EntityFrameworkDataContext>(provider.GetRequiredService<IDataContext>());
            Assert.IsType<ExpenseReportOperations>(provider.GetRequiredService<IExpenseReportOperations>());
        }
    }
}
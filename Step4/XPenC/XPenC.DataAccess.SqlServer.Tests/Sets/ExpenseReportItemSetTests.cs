using System.Linq;
using XPenC.DataAccess.Contracts.Schema;
using XPenC.DataAccess.Contracts.Sets;
using XPenC.DataAccess.SqlServer.Sets;
using XPenC.DataAccess.SqlServer.Tests.TestDoubles;
using Xunit;

namespace XPenC.DataAccess.SqlServer.Tests.Sets
{
    public class ExpenseReportItemSetTests
    {
        private readonly IExpenseReportItemSet _expenseReportItemSet;

        public ExpenseReportItemSetTests()
        {
            _expenseReportItemSet = new ExpenseReportItemSet(new MockSqlDataProvider());
        }

        [Fact]
        public void ExpenseReportItemSet_GetAllFor_ShouldPass()
        {
            var result = _expenseReportItemSet.GetAllFor(1);

            Assert.Equal(2, result.Count());
        }

        [Fact]
        public void ExpenseReportItemSet_AddTo_ShouldPass()
        {
            _expenseReportItemSet.AddTo(1, new ExpenseReportItemEntity());
        }

        [Fact]
        public void ExpenseReportItemSet_DeleteFrom_ShouldPass()
        {
            _expenseReportItemSet.DeleteFrom(1, 1);
        }
    }
}
using System.Linq;
using XPenC.BusinessLogic.Contracts.Models;
using XPenC.DataAccess.Contracts.Exceptions;
using XPenC.DataAccess.Contracts.Sets;
using XPenC.DataAccess.SqlServer.Native.Sets;
using XPenC.DataAccess.SqlServer.Native.Tests.TestDoubles;
using Xunit;

namespace XPenC.DataAccess.SqlServer.Native.Tests.Sets
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
            var result = _expenseReportItemSet.GetAllFor(1).ToArray();

            Assert.Equal(7, result.Length);
        }

        [Fact]
        public void ExpenseReportItemSet_GetAllFor_WithInvalidData_ShouldThrow()
        {
            var expenseReportItemSet = new ExpenseReportItemSet(new MockSqlDataProviderWithInvalidValues());

            Assert.Throws<DataProviderException>(() => expenseReportItemSet.GetAllFor(1));
        }

        [Fact]
        public void ExpenseReportItemSet_AddTo_ShouldPass()
        {
            _expenseReportItemSet.AddTo(1, new ExpenseReportItem());
        }

        [Fact]
        public void ExpenseReportItemSet_DeleteFrom_ShouldPass()
        {
            _expenseReportItemSet.DeleteFrom(1, 1);
        }
    }
}
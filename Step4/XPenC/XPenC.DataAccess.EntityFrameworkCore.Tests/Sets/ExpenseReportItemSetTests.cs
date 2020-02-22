using System.Linq;
using XPenC.DataAccess.Contracts.Schema;
using XPenC.DataAccess.Contracts.Sets;
using XPenC.DataAccess.EntityFrameworkCore.Sets;
using Xunit;

namespace XPenC.DataAccess.EntityFrameworkCore.Tests.Sets
{
    public class ExpenseReportItemSetTests
    {
        private readonly IExpenseReportItemSet _expenseReportItemSet;

        public ExpenseReportItemSetTests()
        {
            var dbContext = new XPenCDbContext(InMemoryDbContextOptionsBuilder<XPenCDbContext>.Build());
            dbContext.ExpenseReports.Add(new ExpenseReportEntity { Id = 1 });
            dbContext.ExpenseReportItems.Add(new ExpenseReportItemEntity { ExpenseReportId = 1, ItemNumber = 1 });
            dbContext.ExpenseReportItems.Add(new ExpenseReportItemEntity { ExpenseReportId = 1, ItemNumber = 2 });
            dbContext.SaveChanges();
            _expenseReportItemSet = new ExpenseReportItemSet(dbContext);
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
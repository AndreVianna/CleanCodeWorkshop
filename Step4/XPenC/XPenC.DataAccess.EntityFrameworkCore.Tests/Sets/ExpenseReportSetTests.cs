using System.Collections.Generic;
using System.Linq;
using XPenC.DataAccess.Contracts.Schema;
using XPenC.DataAccess.Contracts.Sets;
using XPenC.DataAccess.EntityFrameworkCore.Sets;
using Xunit;

namespace XPenC.DataAccess.EntityFrameworkCore.Tests.Sets
{
    public class ExpenseReportSetTests
    {
        private readonly IExpenseReportSet _expenseReportSet;

        public ExpenseReportSetTests()
        {
            var dbContext = new XPenCDbContext(InMemoryDbContextOptionsBuilder<XPenCDbContext>.Build());
            dbContext.ExpenseReports.Add(new ExpenseReportEntity { Id = 1 });
            dbContext.ExpenseReportItems.Add(new ExpenseReportItemEntity { ExpenseReportId = 1, ItemNumber = 1 });
            dbContext.ExpenseReportItems.Add(new ExpenseReportItemEntity { ExpenseReportId = 1, ItemNumber = 2 });
            dbContext.ExpenseReports.Add(new ExpenseReportEntity { Id = 3 });
            dbContext.SaveChanges();
            _expenseReportSet = new ExpenseReportSet(dbContext);
        }

        [Fact]
        public void ExpenseReportSet_GetAll_ShouldPass()
        {
            var result = _expenseReportSet.GetAll();

            Assert.Equal(2, result.Count());
        }

        [Fact]
        public void ExpenseReportSet_Find_WithResultSetFound_ShouldPass()
        {
            var result = _expenseReportSet.Find(1);

            Assert.NotNull(result);
        }

        [Fact]
        public void ExpenseReportSet_Find_WithNoResultSet_ShouldPass()
        {
            var result = _expenseReportSet.Find(2);

            Assert.Null(result);
        }

        [Fact]
        public void ExpenseReportSet_Add_ShouldPass()
        {
            var items = new List<ExpenseReportItemEntity> { new ExpenseReportItemEntity() };
            _expenseReportSet.Add(new ExpenseReportEntity { Items = items });
        }

        [Fact]
        public void ExpenseReportSet_Update_ShouldPass()
        {
            var items = new List<ExpenseReportItemEntity> {new ExpenseReportItemEntity()};
            _expenseReportSet.Update(new ExpenseReportEntity { Items = items });
        }

        [Fact]
        public void ExpenseReportSet_Delete_ShouldPass()
        {
            _expenseReportSet.Delete(1);
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using XPenC.BusinessLogic.Contracts.Models;
using XPenC.DataAccess.Contracts.Sets;
using XPenC.DataAccess.EntityFramework.Schema;
using XPenC.DataAccess.EntityFramework.Sets;
using Xunit;

namespace XPenC.DataAccess.EntityFramework.Tests.Sets
{
    public class ExpenseReportItemSetTests
    {
        private readonly IExpenseReportItemSet _expenseReportItemSet;

        public ExpenseReportItemSetTests()
        {
            var dbContext = new XPenCDbContext(InMemoryDbContextOptionsBuilder<XPenCDbContext>.Build());
            var report = new ExpenseReportEntity {Id = 1};
            dbContext.ExpenseReports.Add(report);
            dbContext.ExpenseReportItems.Add(new ExpenseReportItemEntity { ExpenseReportId = 1, ItemNumber = 1, ExpenseType = "M", });
            dbContext.ExpenseReportItems.Add(new ExpenseReportItemEntity { ExpenseReportId = 1, ItemNumber = 2, ExpenseType = "O", });
            dbContext.SaveChanges();
            _expenseReportItemSet = new ExpenseReportItemSet(dbContext, new List<Action>());
        }

        [Fact]
        public void ExpenseReportItemSet_GetAllFor_ShouldPass()
        {
            var result = _expenseReportItemSet.GetAllFor(1).ToArray();

            Assert.Equal(2, result.Length);
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
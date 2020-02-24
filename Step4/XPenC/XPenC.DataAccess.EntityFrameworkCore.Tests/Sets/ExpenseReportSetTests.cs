using System;
using System.Collections.Generic;
using System.Linq;
using XPenC.BusinessLogic.Contracts.Models;
using XPenC.DataAccess.Contracts.Exceptions;
using XPenC.DataAccess.Contracts.Sets;
using XPenC.DataAccess.EntityFrameworkCore.Schema;
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
            dbContext.ExpenseReportItems.Add(new ExpenseReportItemEntity { ExpenseReportId = 1, ItemNumber = 1, ExpenseType = "M" });
            dbContext.ExpenseReportItems.Add(new ExpenseReportItemEntity { ExpenseReportId = 1, ItemNumber = 2, ExpenseType = "O" });
            dbContext.ExpenseReportItems.Add(new ExpenseReportItemEntity { ExpenseReportId = 1, ItemNumber = 3, ExpenseType = "L" });
            dbContext.ExpenseReportItems.Add(new ExpenseReportItemEntity { ExpenseReportId = 1, ItemNumber = 4, ExpenseType = "L*" });
            dbContext.ExpenseReportItems.Add(new ExpenseReportItemEntity { ExpenseReportId = 1, ItemNumber = 5, ExpenseType = "TL" });
            dbContext.ExpenseReportItems.Add(new ExpenseReportItemEntity { ExpenseReportId = 1, ItemNumber = 6, ExpenseType = "TA" });
            dbContext.ExpenseReportItems.Add(new ExpenseReportItemEntity { ExpenseReportId = 1, ItemNumber = 7, ExpenseType = "Ot" });
            dbContext.ExpenseReports.Add(new ExpenseReportEntity { Id = 3 });
            dbContext.ExpenseReports.Add(new ExpenseReportEntity { Id = 4 });
            dbContext.ExpenseReportItems.Add(new ExpenseReportItemEntity { ExpenseReportId = 4, ItemNumber = 1, ExpenseType = "Invalid" });
            dbContext.SaveChanges();
            _expenseReportSet = new ExpenseReportSet(dbContext, new List<Action>());
        }

        [Fact]
        public void ExpenseReportSet_GetAll_ShouldPass()
        {
            var result = _expenseReportSet.GetAll().ToArray();

            Assert.Equal(3, result.Count());
            Assert.Empty(result[0].Items);
        }

        [Fact]
        public void ExpenseReportSet_Find_WithResultSetFound_ShouldPass()
        {
            var result = _expenseReportSet.Find(1);

            Assert.NotNull(result);
            Assert.NotEmpty(result.Items);
        }

        [Fact]
        public void ExpenseReportSet_Find_WithNoResultSet_ShouldPass()
        {
            var result = _expenseReportSet.Find(2);

            Assert.Null(result);
        }

        [Fact]
        public void ExpenseReportSet_Find_WithInvalidExpenseType_ShouldThrow()
        {
            Assert.Throws<DataProviderException>(() => _expenseReportSet.Find(4));
        }

        [Theory]
        [InlineData(ExpenseType.Office)]
        [InlineData(ExpenseType.Meal)]
        [InlineData(ExpenseType.HotelLodging)]
        [InlineData(ExpenseType.OtherLodging)]
        [InlineData(ExpenseType.LandTransportation)]
        [InlineData(ExpenseType.AirTransportation)]
        [InlineData(ExpenseType.Other)]
        public void ExpenseReportSet_Add_ShouldPass(ExpenseType expenseType)
        {
            var items = new List<ExpenseReportItem> { new ExpenseReportItem { ExpenseType = expenseType } };
            _expenseReportSet.Add(new ExpenseReport { Items = items });
        }

        [Fact]
        public void ExpenseReportSet_Update_ShouldPass()
        {
            var report = _expenseReportSet.Find(1);
            _expenseReportSet.Update(report);
        }

        [Fact]
        public void ExpenseReportSet_Delete_ShouldPass()
        {
            _expenseReportSet.Delete(1);
        }
    }
}
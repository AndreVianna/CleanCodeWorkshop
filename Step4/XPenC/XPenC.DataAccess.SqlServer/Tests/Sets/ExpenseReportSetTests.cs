using System.Collections.Generic;
using System.Linq;
using XPenC.BusinessLogic.Contracts.Models;
using XPenC.DataAccess.Contracts.Sets;
using XPenC.DataAccess.SqlServer.Sets;
using XPenC.DataAccess.SqlServer.Tests.TestDoubles;
using Xunit;

namespace XPenC.DataAccess.SqlServer.Tests.Sets
{
    public class ExpenseReportSetTests
    {
        private readonly IExpenseReportSet _expenseReportSet;

        public ExpenseReportSetTests()
        {
            var expenseReportItemSet = new ExpenseReportItemSet(new MockSqlDataProvider());
            _expenseReportSet = new ExpenseReportSet(new MockSqlDataProvider(), expenseReportItemSet);
        }

        [Fact]
        public void ExpenseReportSet_GetAll_ShouldPass()
        {
            var result = _expenseReportSet.GetAll().ToArray();

            Assert.Equal(2, result.Count());
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
            var items = new List<ExpenseReportItem> {new ExpenseReportItem()};
            _expenseReportSet.Update(new ExpenseReport { Items = items });
        }

        [Fact]
        public void ExpenseReportSet_Delete_ShouldPass()
        {
            _expenseReportSet.Delete(1);
        }
    }
}
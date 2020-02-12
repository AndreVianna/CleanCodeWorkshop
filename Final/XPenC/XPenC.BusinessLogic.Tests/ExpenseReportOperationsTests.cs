using System;
using XPenC.BusinessLogic.Contracts.Models;
using XPenC.DataAccess.Contracts.Schema;
using Xunit;

namespace XPenC.BusinessLogic.Tests
{
    public class ExpenseReportOperationsTests
    {
        private readonly InMemoryDataContext _inMemoryDataContext;
        private readonly ExpenseReportOperations _expenseReportOperations;

        public ExpenseReportOperationsTests()
        {
            _inMemoryDataContext = new InMemoryDataContext();
            _expenseReportOperations = new ExpenseReportOperations(_inMemoryDataContext);
        }

        [Fact]
        public void ExpenseReportOperations_Add_ShouldPass()
        {
            var item = new ExpenseReport();

            _expenseReportOperations.Add(item);

            Assert.Equal(1, item.Id);
            Assert.NotNull(_inMemoryDataContext.ExpenseReports.Find(item.Id));
        }

        [Fact]
        public void ExpenseReportOperations_Find_ShouldPass()
        {
            _inMemoryDataContext.ExpenseReports.Add(new ExpenseReportEntity { Id = 1 });
            _inMemoryDataContext.ExpenseReports.Add(new ExpenseReportEntity { Id = 2 });

            var subject = _expenseReportOperations.Find(2);

            Assert.NotNull(subject);
            Assert.Equal(2, subject.Id);
        }

        [Fact]
        public void ExpenseReportOperations_Find_WithInvalidId_ShouldPass()
        {
            _inMemoryDataContext.ExpenseReports.Add(new ExpenseReportEntity { Id = 1 });
            _inMemoryDataContext.ExpenseReports.Add(new ExpenseReportEntity { Id = 2 });

            var subject = _expenseReportOperations.Find(3);

            Assert.Null(subject);
        }

        [Fact]
        public void ExpenseReportOperations_CreateWithDefaults_ShouldPass()
        {
            var subject = _expenseReportOperations.CreateWithDefaults();

            var now = DateTime.Now;
            Assert.NotNull(subject);
            Assert.Equal(0, subject.Id);
            Assert.Equal(0, subject.MealTotal);
            Assert.Equal(0, subject.Total);
            Assert.Null(subject.Client);
            Assert.InRange(subject.CreatedOn, now.AddMilliseconds(-50), now);
            Assert.InRange(subject.ModifiedOn, now.AddMilliseconds(-50), now);
        }

        [Fact]
        public void ExpenseReportOperations_GetList_ShouldPass()
        {
            _inMemoryDataContext.ExpenseReports.Add(new ExpenseReportEntity { Id = 1 });
            _inMemoryDataContext.ExpenseReports.Add(new ExpenseReportEntity { Id = 2 });

            var subject = _expenseReportOperations.GetList();

            Assert.NotNull(subject);
            Assert.Equal(2, subject.Count);
        }
    }
}

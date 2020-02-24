using System;
using System.Collections.Generic;
using System.Linq;
using XPenC.BusinessLogic.Contracts.Exceptions;
using XPenC.BusinessLogic.Contracts.Models;
using XPenC.BusinessLogic.Tests.TestDoubles;
using Xunit;
using static XPenC.BusinessLogic.Contracts.Models.ExpenseType;

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
        public void ExpenseReportOperations_CreateWithDefaults_ShouldPass()
        {
            var subject = _expenseReportOperations.CreateWithDefaults();

            var now = DateTime.Now;
            Assert.NotNull(subject);
            Assert.Equal(0, subject.Id);
            Assert.Equal(0, subject.MealTotal);
            Assert.Equal(0, subject.Total);
            Assert.Null(subject.Client);
            Assert.Equal(now, subject.CreatedOn, TimeSpan.FromMilliseconds(50));
            Assert.Equal(now, subject.ModifiedOn, TimeSpan.FromMilliseconds(50));
        }

        [Fact]
        public void ExpenseReportOperations_Add_WithEmptyClient_ShouldPass()
        {
            var item = new ExpenseReport();

            var exception = Assert.Throws<ValidationException>(() => _expenseReportOperations.Add(item));
            Assert.Equal("Add", exception.Operation);
            Assert.Single(exception.Errors);
            Assert.Equal("Client", exception.Errors.First().Source);
            Assert.Equal("The 'Client' field is required.", exception.Errors.First().Message);
        }


        [Fact]
        public void ExpenseReportOperations_Add_ShouldPass()
        {
            var item = new ExpenseReport { Client = "Client 3" };

            _expenseReportOperations.Add(item);

            Assert.Equal(1, item.Id);
            Assert.NotNull(_inMemoryDataContext.ExpenseReports.Find(item.Id));
        }

        [Fact]
        public void ExpenseReportOperations_Find_ShouldPass()
        {
            var createdDate = DateTime.Now.AddDays(-1);
            _inMemoryDataContext.ExpenseReports.Add(new ExpenseReport
            {
                Id = 1,
                Client = "Value",
                CreatedOn = createdDate,
                ModifiedOn = createdDate.AddMinutes(1),
                Items = new List<ExpenseReportItem>
                {
                    new ExpenseReportItem { ExpenseReportId = 1, ItemNumber = 1, ExpenseType = Office, Date = createdDate.Date.AddDays(-2), Value = 10 },
                    new ExpenseReportItem{ ExpenseReportId = 1, ItemNumber = 2, ExpenseType = Meal, Date = createdDate.Date.AddDays(-3), Value = 20 }
                }
            });

            var subject = _expenseReportOperations.Find(1);

            Assert.NotNull(subject);
            Assert.Equal(1, subject.Id);
        }

        [Fact]
        public void ExpenseReportOperations_Find_WithInvalidId_ShouldPass()
        {
            _inMemoryDataContext.ExpenseReports.Add(new ExpenseReport { Id = 1 });
            _inMemoryDataContext.ExpenseReports.Add(new ExpenseReport { Id = 2 });

            var subject = _expenseReportOperations.Find(3);

            Assert.Null(subject);
        }

        [Fact]
        public void ExpenseReportOperations_GetList_ShouldPass()
        {
            _inMemoryDataContext.ExpenseReports.Add(new ExpenseReport { Id = 1 });
            _inMemoryDataContext.ExpenseReports.Add(new ExpenseReport { Id = 2 });

            var subject = _expenseReportOperations.GetList();

            Assert.NotNull(subject);
            Assert.Equal(2, subject.ToArray().Length);
        }

        [Fact]
        public void ExpenseReportOperations_Delete_ShouldPass()
        {
            _inMemoryDataContext.ExpenseReports.Add(new ExpenseReport { Id = 1 });
            _inMemoryDataContext.ExpenseReports.Add(new ExpenseReport { Id = 2 });

            _expenseReportOperations.Delete(2);

            var subject = _inMemoryDataContext.ExpenseReports.Find(2);

            Assert.Null(subject);
        }

        [Fact]
        public void ExpenseReportOperations_Add_WithItems_ShouldPass()
        {
            var newReport = new ExpenseReport
            {
                Client = "Client 3",
                Items = new List<ExpenseReportItem>
                {
                    new ExpenseReportItem { ExpenseType = Meal, Value = 10 },
                    new ExpenseReportItem { ExpenseType = HotelLodging, Value = 1000 },
                    new ExpenseReportItem { ExpenseType = Meal, Value = 1000 },
                }
            };

            _expenseReportOperations.Add(newReport);

            Assert.Equal(1, newReport.Id);
            Assert.Equal(3, newReport.Items.Count);
            Assert.Equal(1010, newReport.MealTotal);
            Assert.Equal(2010, newReport.Total);
            AssertReportItem(newReport, 1, false);
            AssertReportItem(newReport, 2, false);
            AssertReportItem(newReport, 3, true);
        }

        private static void AssertReportItem(ExpenseReport report, int expectedItemNumber, bool expectedToBeAboveMaximum)
        {
            var item = report.Items.FirstOrDefault(i => i.ItemNumber == expectedItemNumber);
            Assert.NotNull(item);
            Assert.Equal(expectedItemNumber, item.ItemNumber);
            Assert.Equal(expectedToBeAboveMaximum, item.IsAboveMaximum);
        }

        [Fact]
        public void ExpenseReportOperations_Update_ShouldPass()
        {
            var createdDate = DateTime.Now.AddDays(-1);
            var originalReport = new ExpenseReport
            {
                Id = 1,
                Client = "Old Value",
                CreatedOn = createdDate,
                ModifiedOn = createdDate.AddMinutes(1),
                Items = new List<ExpenseReportItem>
                {
                    new ExpenseReportItem { ExpenseReportId = 1, ItemNumber = 1, ExpenseType = Office, Date = createdDate.Date.AddDays(-2), Value = 10 },
                    new ExpenseReportItem{ ExpenseReportId = 1, ItemNumber = 2, ExpenseType = Meal, Date = createdDate.Date.AddDays(-3), Value = 20 }
                }
            };
            _inMemoryDataContext.ExpenseReports.Add(originalReport);
            var subject = _expenseReportOperations.Find(1);
            var itemsBefore = subject.Items.ToList();

            subject.Client = "New Value";
            subject.Items.Remove(itemsBefore.Find(i => i.ItemNumber == 1));
            subject.Items.Add(new ExpenseReportItem());

            _expenseReportOperations.Update(subject);

            var now = DateTime.Now;
            var updatedReport = _expenseReportOperations.Find(1);
            var itemsAfter = updatedReport.Items.ToList();
            Assert.Equal("New Value", updatedReport.Client);
            Assert.Equal(2, itemsAfter.Count);
            Assert.Null(itemsAfter.Find(i => i.ItemNumber == 1));
            Assert.NotNull(itemsAfter.Find(i => i.ItemNumber == 2));
            Assert.NotNull(itemsAfter.Find(i => i.ItemNumber == 3));
            Assert.Equal(now, subject.ModifiedOn, TimeSpan.FromMilliseconds(50));
            Assert.Equal(createdDate, subject.CreatedOn);
        }

        //[Fact]
        //public void ExpenseReportOperations_CalculateReportMealTotal_ForEmptyList_ShouldPass()
        //{
        //    var items = Enumerable.Empty<ExpenseReportItem>();
            
        //    var result = ExpenseReportOperations.CalculateReportMealTotal(items);

        //    Assert.Equal(0 , result);
        //}

        //[Fact]
        //public void ExpenseReportOperations_CalculateReportMealTotal_ShouldPass()
        //{
        //    var items = new List<ExpenseReportItem>
        //    {
        //        new ExpenseReportItem { ExpenseType = Other, Value = 100 },
        //        new ExpenseReportItem { ExpenseType = Meal, Value = 20 },
        //        new ExpenseReportItem { ExpenseType = Meal, Value = 10 },
        //    };

        //    var result = ExpenseReportOperations.CalculateReportMealTotal(items);

        //    Assert.Equal(30, result);
        //}

        //[Fact]
        //public void ExpenseReportOperations_CalculateReportTotal_ForEmptyList_ShouldPass()
        //{
        //    var items = Enumerable.Empty<ExpenseReportItem>();

        //    var result = ExpenseReportOperations.CalculateReportTotal(items);

        //    Assert.Equal(0, result);
        //}

        //[Fact]
        //public void ExpenseReportOperations_CalculateReportTotal_ShouldPass()
        //{
        //    var items = new List<ExpenseReportItem>
        //    {
        //        new ExpenseReportItem { ExpenseType = Other, Value = 100 },
        //        new ExpenseReportItem { ExpenseType = Meal, Value = 20 },
        //        new ExpenseReportItem { ExpenseType = Meal, Value = 10 },
        //    };

        //    var result = ExpenseReportOperations.CalculateReportTotal(items);

        //    Assert.Equal(130, result);
        //}

        //[Fact]
        //public void ExpenseReportOperations_IsExpenseAboveMaximum_ForDefaultItem_ShouldPass()
        //{
        //    var items = new ExpenseReportItem();

        //    var result = ExpenseReportOperations.IsExpenseAboveMaximum(items);

        //    Assert.False(result);
        //}

        //[Fact]
        //public void ExpenseReportOperations_IsExpenseAboveMaximum_ForNonMealItem_ShouldPass()
        //{
        //    var items = new ExpenseReportItem { ExpenseType = Other, Value = 10000 };

        //    var result = ExpenseReportOperations.IsExpenseAboveMaximum(items);

        //    Assert.False(result);
        //}

        //[Fact]
        //public void ExpenseReportOperations_IsExpenseAboveMaximum_ForLowValueItem_ShouldPass()
        //{
        //    var items = new ExpenseReportItem { ExpenseType = Meal, Value = 10 };

        //    var result = ExpenseReportOperations.IsExpenseAboveMaximum(items);

        //    Assert.False(result);
        //}

        //[Fact]
        //public void ExpenseReportOperations_IsExpenseAboveMaximum_ForAboveMaximumItem_ShouldPass()
        //{
        //    var items = new ExpenseReportItem { ExpenseType = Meal, Value = 10000 };

        //    var result = ExpenseReportOperations.IsExpenseAboveMaximum(items);

        //    Assert.True(result);
        //}
    }
}

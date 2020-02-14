using System;
using System.Collections.Generic;
using System.Linq;
using XPenC.BusinessLogic.Contracts.Models;
using XPenC.BusinessLogic.Tests.TestDoubles;
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
        public void ExpenseReportOperations_MaximumMealValue_ShouldPass()
        {
            Assert.Equal(50m, ExpenseReportOperations.MaximumMealValue);
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
        public void ExpenseReportOperations_GetList_ShouldPass()
        {
            _inMemoryDataContext.ExpenseReports.Add(new ExpenseReportEntity { Id = 1 });
            _inMemoryDataContext.ExpenseReports.Add(new ExpenseReportEntity { Id = 2 });

            var subject = _expenseReportOperations.GetList();

            Assert.NotNull(subject);
            Assert.Equal(2, subject.ToArray().Length);
        }

        [Fact]
        public void ExpenseReportOperations_Delete_ShouldPass()
        {
            _inMemoryDataContext.ExpenseReports.Add(new ExpenseReportEntity { Id = 1 });
            _inMemoryDataContext.ExpenseReports.Add(new ExpenseReportEntity { Id = 2 });

            _expenseReportOperations.Delete(2);

            var subject = _inMemoryDataContext.ExpenseReports.Find(2);

            Assert.Null(subject);
        }

        [Fact]
        public void ExpenseReportOperations_Add_WithItems_ShouldPass()
        {
            var newReport = new ExpenseReport
            {
                Items = new List<ExpenseReportItem>
                {
                    new ExpenseReportItem { ExpenseType = ExpenseType.Meal, Value = 10 },
                    new ExpenseReportItem { ExpenseType = ExpenseType.HotelLodging, Value = 1000 },
                    new ExpenseReportItem { ExpenseType = ExpenseType.Meal, Value = 1000 },
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
        public void ExpenseReportOperations_AddDefaultItem_ShouldPass()
        {
            TestAddExpenseReportTypeItem(new ExpenseReportItem());
        }

        [Fact]
        public void ExpenseReportOperations_AddMealItem_ShouldPass()
        {
            TestAddExpenseReportTypeItem(new ExpenseReportItem { ExpenseType = ExpenseType.Meal });
        }

        [Fact]
        public void ExpenseReportOperations_AddAirTransportationItem_ShouldPass()
        {
            TestAddExpenseReportTypeItem(new ExpenseReportItem { ExpenseType = ExpenseType.AirTransportation });
        }

        [Fact]
        public void ExpenseReportOperations_AddHotelLodgingItem_ShouldPass()
        {
            TestAddExpenseReportTypeItem(new ExpenseReportItem { ExpenseType = ExpenseType.HotelLodging });
        }

        [Fact]
        public void ExpenseReportOperations_AddLandTransportationItem_ShouldPass()
        {
            TestAddExpenseReportTypeItem(new ExpenseReportItem { ExpenseType = ExpenseType.LandTransportation });
        }

        [Fact]
        public void ExpenseReportOperations_AddOfficeItem_ShouldPass()
        {
            TestAddExpenseReportTypeItem(new ExpenseReportItem { ExpenseType = ExpenseType.Office });
        }

        [Fact]
        public void ExpenseReportOperations_AddOtherLodgingItem_ShouldPass()
        {
            TestAddExpenseReportTypeItem(new ExpenseReportItem { ExpenseType = ExpenseType.OtherLodging });
        }

        [Fact]
        public void ExpenseReportOperations_AddOtherItem_ShouldPass()
        {
            TestAddExpenseReportTypeItem(new ExpenseReportItem { ExpenseType = ExpenseType.Other });
        }

        private void TestAddExpenseReportTypeItem(ExpenseReportItem newItem)
        {
            _inMemoryDataContext.ExpenseReports.Add(new ExpenseReportEntity {Id = 1});
            var subject = _expenseReportOperations.Find(1);
            var itemsBefore = subject.Items.ToArray();

            _expenseReportOperations.AddItem(subject, newItem);

            var updatedReport = _inMemoryDataContext.ExpenseReports.Find(1);
            var itemsAfter = updatedReport.Items.ToList();
            Assert.Empty(itemsBefore);
            Assert.Single(itemsAfter);
            var insertedItem = itemsAfter[0];
            Assert.Equal(updatedReport.Id, insertedItem.ExpenseReportId);
            Assert.Equal(1, insertedItem.ItemNumber);
        }

        [Fact]
        public void ExpenseReportOperations_RemoveItem_ShouldPass()
        {
            var expenseReportWithItems = new ExpenseReportEntity
            {
                Id = 1,
                Items = new List<ExpenseReportItemEntity>
                {
                    new ExpenseReportItemEntity { ExpenseReportId = 1, ItemNumber = 1, },
                    new ExpenseReportItemEntity{ ExpenseReportId = 1, ItemNumber = 2, }
                }
            };
            _inMemoryDataContext.ExpenseReports.Add(expenseReportWithItems);
            var subject = _expenseReportOperations.Find(1);
            var itemsBefore = subject.Items.ToArray();

            _expenseReportOperations.RemoveItem(subject, 1);

            var updatedReport = _expenseReportOperations.Find(1);
            var itemsAfter = updatedReport.Items.ToList();
            Assert.Equal(2, itemsBefore.Length);
            Assert.Single(itemsAfter);
            Assert.Null(itemsAfter.Find(i => i.ItemNumber == 1));
            Assert.NotNull(itemsAfter.Find(i => i.ItemNumber == 2));
        }

        [Fact]
        public void ExpenseReportOperations_RemoveNotExistingItem_ShouldPass()
        {
            var expenseReportWithItems = new ExpenseReportEntity
            {
                Id = 1,
                Items = new List<ExpenseReportItemEntity>
                {
                    new ExpenseReportItemEntity { ExpenseReportId = 1, ItemNumber = 1, },
                    new ExpenseReportItemEntity{ ExpenseReportId = 1, ItemNumber = 2, }
                }
            };
            _inMemoryDataContext.ExpenseReports.Add(expenseReportWithItems);
            var subject = _expenseReportOperations.Find(1);
            var itemsBefore = subject.Items.ToArray();

            _expenseReportOperations.RemoveItem(subject, 3);

            var updatedReport = _expenseReportOperations.Find(1);
            var itemsAfter = updatedReport.Items.ToList();
            Assert.Equal(itemsBefore.Length, itemsAfter.Count);
            Assert.NotNull(itemsAfter.Find(i => i.ItemNumber == 1));
            Assert.NotNull(itemsAfter.Find(i => i.ItemNumber == 2));
        }

        [Fact]
        public void ExpenseReportOperations_AddItem_IntoReportWithItems_ShouldPass()
        {
            var expenseReportWithItems = new ExpenseReportEntity
            {
                Id = 1,
                Items = new List<ExpenseReportItemEntity>
                {
                    new ExpenseReportItemEntity{ ExpenseReportId = 1, ItemNumber = 2, }
                }
            };
            _inMemoryDataContext.ExpenseReports.Add(expenseReportWithItems);
            var originalReport = _expenseReportOperations.Find(1);
            var itemsBefore = originalReport.Items.ToArray();

            _expenseReportOperations.AddItem(originalReport, new ExpenseReportItem());

            var updatedReport = _expenseReportOperations.Find(1);
            var itemsAfter = updatedReport.Items.ToList();
            Assert.Single(itemsBefore);
            Assert.Equal(2, itemsAfter.Count);
            var insertedItem = itemsAfter[1];
            Assert.Equal(updatedReport.Id, insertedItem.ExpenseReportId);
            Assert.Equal(3, insertedItem.ItemNumber);
        }

        [Fact]
        public void ExpenseReportOperations_Update_ShouldPass()
        {
            var createdDate = DateTime.Now.AddDays(-1);
            var originalReport = new ExpenseReportEntity
            {
                Id = 1,
                Client = "Old Value",
                CreatedOn = createdDate,
                ModifiedOn = createdDate.AddMinutes(1),
                Items = new List<ExpenseReportItemEntity>
                {
                    new ExpenseReportItemEntity { ExpenseReportId = 1, ItemNumber = 1, },
                    new ExpenseReportItemEntity{ ExpenseReportId = 1, ItemNumber = 2, }
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
            Assert.InRange(subject.ModifiedOn, now.AddMilliseconds(-50), now);
            Assert.Equal(createdDate, subject.CreatedOn);
        }

        [Fact]
        public void ExpenseReportOperations_CalculateReportMealTotal_ForEmptyList_ShouldPass()
        {
            var items = Enumerable.Empty<ExpenseReportItem>();
            
            var result = ExpenseReportOperations.CalculateReportMealTotal(items);

            Assert.Equal(0 , result);
        }

        [Fact]
        public void ExpenseReportOperations_CalculateReportMealTotal_ShouldPass()
        {
            var items = new List<ExpenseReportItem>
            {
                new ExpenseReportItem { ExpenseType = ExpenseType.Other, Value = 100 },
                new ExpenseReportItem { ExpenseType = ExpenseType.Meal, Value = 20 },
                new ExpenseReportItem { ExpenseType = ExpenseType.Meal, Value = 10 },
            };

            var result = ExpenseReportOperations.CalculateReportMealTotal(items);

            Assert.Equal(30, result);
        }

        [Fact]
        public void ExpenseReportOperations_CalculateReportTotal_ForEmptyList_ShouldPass()
        {
            var items = Enumerable.Empty<ExpenseReportItem>();

            var result = ExpenseReportOperations.CalculateReportTotal(items);

            Assert.Equal(0, result);
        }

        [Fact]
        public void ExpenseReportOperations_CalculateReportTotal_ShouldPass()
        {
            var items = new List<ExpenseReportItem>
            {
                new ExpenseReportItem { ExpenseType = ExpenseType.Other, Value = 100 },
                new ExpenseReportItem { ExpenseType = ExpenseType.Meal, Value = 20 },
                new ExpenseReportItem { ExpenseType = ExpenseType.Meal, Value = 10 },
            };

            var result = ExpenseReportOperations.CalculateReportTotal(items);

            Assert.Equal(130, result);
        }

        [Fact]
        public void ExpenseReportOperations_IsExpenseAboveMaximum_ForDefaultItem_ShouldPass()
        {
            var items = new ExpenseReportItem();

            var result = ExpenseReportOperations.IsExpenseAboveMaximum(items);

            Assert.False(result);
        }

        [Fact]
        public void ExpenseReportOperations_IsExpenseAboveMaximum_ForNonMealItem_ShouldPass()
        {
            var items = new ExpenseReportItem { ExpenseType = ExpenseType.Other, Value = 10000 };

            var result = ExpenseReportOperations.IsExpenseAboveMaximum(items);

            Assert.False(result);
        }

        [Fact]
        public void ExpenseReportOperations_IsExpenseAboveMaximum_ForLowValueItem_ShouldPass()
        {
            var items = new ExpenseReportItem { ExpenseType = ExpenseType.Meal, Value = 10 };

            var result = ExpenseReportOperations.IsExpenseAboveMaximum(items);

            Assert.False(result);
        }

        [Fact]
        public void ExpenseReportOperations_IsExpenseAboveMaximum_ForAboveMaximumItem_ShouldPass()
        {
            var items = new ExpenseReportItem { ExpenseType = ExpenseType.Meal, Value = 10000 };

            var result = ExpenseReportOperations.IsExpenseAboveMaximum(items);

            Assert.True(result);
        }
    }
}

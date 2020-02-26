using System;
using System.Linq;
using XPenC.BusinessLogic.Contracts.Exceptions;
using XPenC.BusinessLogic.Contracts.Models;
using XPenC.BusinessLogic.Tests.TestDoubles;
using Xunit;

namespace XPenC.BusinessLogic.Tests
{
    public class ExpenseReportItemOperationsTests
    {
        private readonly InMemoryDataContext _inMemoryDataContext;
        private readonly ExpenseReportItemOperations _expenseReportItemOperations;

        public ExpenseReportItemOperationsTests()
        {
            _inMemoryDataContext = new InMemoryDataContext();
            _expenseReportItemOperations = new ExpenseReportItemOperations(_inMemoryDataContext);
        }

        [Fact]
        public void ExpenseReportItemOperations_Add_WithZeroValue_ShouldThrow()
        {
            var item = new ExpenseReportItem { ExpenseReportId = 1, Value = 0 };

            var exception = Assert.Throws<ValidationException>(() => _expenseReportItemOperations.Add(item));
            Assert.Equal("Add", exception.Operation);
            Assert.Single(exception.Errors);
            Assert.Equal("Value", exception.Errors.First().Source);
            Assert.Equal("The expense item value must be grater than zero.", exception.Errors.First().Message);
        }

        [Fact]
        public void ExpenseReportItemOperations_Add_WithNegativeValue_ShouldThrow()
        {
            var item = new ExpenseReportItem { ExpenseReportId = 1, Value = -1 };

            var exception = Assert.Throws<ValidationException>(() => _expenseReportItemOperations.Add(item));
            Assert.Equal("Add", exception.Operation);
            Assert.Single(exception.Errors);
            Assert.Equal("Value", exception.Errors.First().Source);
            Assert.Equal("The expense item value must be grater than zero.", exception.Errors.First().Message);
        }

        [Fact]
        public void ExpenseReportItemOperations_Add_WithFutureDate_ShouldThrow()
        {
            var item = new ExpenseReportItem { ExpenseReportId = 1, Date = DateTime.Now.AddDays(2), Value = 10 };

            var exception = Assert.Throws<ValidationException>(() => _expenseReportItemOperations.Add(item));
            Assert.Equal("Add", exception.Operation);
            Assert.Single(exception.Errors);
            Assert.Equal("Date", exception.Errors.First().Source);
            Assert.Equal("The expense item date must not be in the future.", exception.Errors.First().Message);
        }

        [Fact]
        public void ExpenseReportItemOperations_Add_WithInvalidDateAndValue_ShouldThrow()
        {
            var item = new ExpenseReportItem { ExpenseReportId = 1, Date = DateTime.Now.AddDays(2), Value = -1 };

            var exception = Assert.Throws<ValidationException>(() => _expenseReportItemOperations.Add(item));
            Assert.Equal("Add", exception.Operation);
            Assert.Equal(2, exception.Errors.Count());
            Assert.Equal("Date", exception.Errors.ElementAt(0).Source);
            Assert.Equal("The expense item date must not be in the future.", exception.Errors.ElementAt(0).Message);
            Assert.Equal("Value", exception.Errors.ElementAt(1).Source);
            Assert.Equal("The expense item value must be grater than zero.", exception.Errors.ElementAt(1).Message);
        }

        [Theory]
        [InlineData(ExpenseType.Office)]
        [InlineData(ExpenseType.Meal)]
        [InlineData(ExpenseType.HotelLodging)]
        [InlineData(ExpenseType.OtherLodging)]
        [InlineData(ExpenseType.LandTransportation)]
        [InlineData(ExpenseType.AirTransportation)]
        [InlineData(ExpenseType.Other)]
        public void ExpenseReportItemOperations_Add_ShouldPass(ExpenseType expenseType)
        {
            var item = new ExpenseReportItem { ExpenseReportId = 1, ExpenseType = expenseType, Value = 10 };

            _expenseReportItemOperations.Add(item);

            Assert.Equal(1, item.ItemNumber);
            var items = _inMemoryDataContext.ExpenseReportItems.GetAllFor(1).ToList();
            Assert.NotNull(items.Find(i => i.ItemNumber == item.ItemNumber));
        }

        [Fact]
        public void ExpenseReportItemOperations_Delete_ShouldPass()
        {
            _inMemoryDataContext.ExpenseReportItems.AddTo(1, new ExpenseReportItem { ExpenseReportId = 1, ItemNumber = 1 });
            _inMemoryDataContext.ExpenseReportItems.AddTo(1, new ExpenseReportItem { ExpenseReportId = 1, ItemNumber = 2 });

            _expenseReportItemOperations.Delete(1, 2);

            var items = _inMemoryDataContext.ExpenseReportItems.GetAllFor(1).ToList();
            Assert.Null(items.Find(i => i.ItemNumber == 2));
        }
    }
}
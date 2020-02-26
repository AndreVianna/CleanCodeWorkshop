using System;
using System.Collections.Generic;
using XPenC.BusinessLogic.Contracts.Models;

namespace XPenC.WebApp.Tests.TestDoubles
{
    internal  class FakeExpenseReportOperations : DummyExpenseReportOperations
    {
        public static ExpenseReport ExistingReport1 { get; } = new ExpenseReport
        {
            Id = 1,
            Client = "Some Client",
            Items = new List<ExpenseReportItem>
            {
                new ExpenseReportItem { ExpenseReportId = 1, ItemNumber = 1, ExpenseType = ExpenseType.Meal, Date = DateTime.Now.AddDays(-1), Value = 10, Description = "Some Description" },
                new ExpenseReportItem { ExpenseReportId = 1, ItemNumber = 2, ExpenseType = ExpenseType.Meal, Date = DateTime.Now.AddDays(-1), Value = 60, IsAboveMaximum = true },
                new ExpenseReportItem { ExpenseReportId = 1, ItemNumber = 3, ExpenseType = ExpenseType.HotelLodging, Date = DateTime.Now.AddDays(-1), Value = 100, Description = "Other Description" },
                new ExpenseReportItem { ExpenseReportId = 1, ItemNumber = 3, ExpenseType = ExpenseType.Office, Date = DateTime.Now.AddDays(-1), Value = 1 },
                new ExpenseReportItem { ExpenseReportId = 1, ItemNumber = 3, ExpenseType = ExpenseType.OtherLodging, Date = DateTime.Now.AddDays(-1), Value = 1 },
                new ExpenseReportItem { ExpenseReportId = 1, ItemNumber = 3, ExpenseType = ExpenseType.LandTransportation, Date = DateTime.Now.AddDays(-1), Value = 1 },
                new ExpenseReportItem { ExpenseReportId = 1, ItemNumber = 3, ExpenseType = ExpenseType.AirTransportation, Date = DateTime.Now.AddDays(-1), Value = 1 },
                new ExpenseReportItem { ExpenseReportId = 1, ItemNumber = 3, ExpenseType = ExpenseType.Other, Date = DateTime.Now.AddDays(-1), Value = 1 },
            },
            MealTotal = 70,
            Total = 175,
            CreatedOn = DateTime.Now.AddDays(-2),
            ModifiedOn = DateTime.Now.AddDays(-1),
        };
        private static ExpenseReport ExistingReport2 { get; } = new ExpenseReport
        {
            Id = 2,
            Client = "Other Client",
            CreatedOn = DateTime.Now.AddDays(-1),
            ModifiedOn = DateTime.Now.AddDays(-1),
        };

        public override IEnumerable<ExpenseReport> GetList()
        {
            return new List<ExpenseReport> {ExistingReport1, ExistingReport2};
        }

        public override ExpenseReport Find(int id)
        {
            return id == ExistingReport1.Id 
                ? ExistingReport1
                : id == ExistingReport2.Id 
                    ? ExistingReport2
                    : null;
        }

        public override ExpenseReport CreateWithDefaults()
        {
            return new ExpenseReport();
        }

        public Action ExpectedAddBehavior { get; set; }
        public override void Add(ExpenseReport newExpenseReport)
        {
            ExpectedAddBehavior?.Invoke();
        }

        public override void AddItem(ExpenseReport source, ExpenseReportItem newItem)
        {
        }

        public override void RemoveItem(ExpenseReport source, int itemNumber)
        {
        }

        public Action ExpectedUpdateBehavior { get; set; }
        public override void Update(ExpenseReport source)
        {
            ExpectedUpdateBehavior?.Invoke();
        }

        public override void Delete(int id)
        {
        }
    }
}
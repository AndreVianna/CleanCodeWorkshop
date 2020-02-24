using System;
using XPenC.BusinessLogic.Contracts.Models;

namespace XPenC.WebApp.Tests.TestDoubles
{
    internal class FakeExpenseReportItemOperations : DummyExpenseReportItemOperations
    {
        public Action ExpectedAddBehavior { get; set; }
        public override void Add(ExpenseReportItem newExpenseReportItem)
        {
            ExpectedAddBehavior?.Invoke();
        }

        public override void Delete(int expenseReportId, int itemNumber)
        {
        }
    }
}
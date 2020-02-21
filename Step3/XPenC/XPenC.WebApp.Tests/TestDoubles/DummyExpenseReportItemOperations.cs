using XPenC.BusinessLogic.Contracts;
using XPenC.BusinessLogic.Contracts.Models;

namespace XPenC.WebApp.Tests.TestDoubles
{
    internal class DummyExpenseReportItemOperations : IExpenseReportItemOperations
    {
        public virtual void Add(int expenseReportId, ExpenseReportItem newExpenseReportItem) => throw new System.NotImplementedException();
        public virtual void Delete(int expenseReportId, int itemNumber) => throw new System.NotImplementedException();
    }
}
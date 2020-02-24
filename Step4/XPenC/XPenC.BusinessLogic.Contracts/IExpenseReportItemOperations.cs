using XPenC.BusinessLogic.Contracts.Models;

namespace XPenC.BusinessLogic.Contracts
{
    public interface IExpenseReportItemOperations
    {
        void Add(ExpenseReportItem newExpenseReportItem);
        void Delete(int expenseReportId, int itemNumber);
    }
}
using System.Collections.Generic;
using XPenC.BusinessLogic.Contracts.Models;

namespace XPenC.BusinessLogic.Contracts
{
    public interface IExpenseReportOperations
    {
        ExpenseReport CreateWithDefaults();

        List<ExpenseReport> GetList();
        void Add(ExpenseReport newExpenseReport);
        ExpenseReport Find(int id);
        void Update(ExpenseReport source);
        void Delete(int id);

        void AddItem(ExpenseReport source, ExpenseReportItem newItemEntity);
        void RemoveItem(ExpenseReport source, int itemNumber);
    }
}
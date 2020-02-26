using System.Collections.Generic;
using XPenC.BusinessLogic.Contracts.Models;

namespace XPenC.BusinessLogic.Contracts
{
    public interface IExpenseReportOperations
    {
        ExpenseReport CreateWithDefaults();

        IEnumerable<ExpenseReport> GetList();
        void Add(ExpenseReport newExpenseReport);
        ExpenseReport Find(int id);
        void Update(ExpenseReport source);
        void Delete(int id);
    }
}
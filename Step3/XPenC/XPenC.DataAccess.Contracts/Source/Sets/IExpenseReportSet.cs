using System.Collections.Generic;
using XPenC.BusinessLogic.Contracts.Models;

namespace XPenC.DataAccess.Contracts.Sets
{
    public interface IExpenseReportSet
    {
        IEnumerable<ExpenseReport> GetAll();
        ExpenseReport Find(int id);

        void Add(ExpenseReport source);
        void Update(ExpenseReport source);
        void Delete(int id);
    }
}
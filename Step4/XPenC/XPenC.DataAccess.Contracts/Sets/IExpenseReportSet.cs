using System.Collections.Generic;
using XPenC.DataAccess.Contracts.Schema;

namespace XPenC.DataAccess.Contracts.Sets
{
    public interface IExpenseReportSet
    {
        IEnumerable<ExpenseReportEntity> GetAll();
        ExpenseReportEntity Find(int id);

        void Add(ExpenseReportEntity source);
        void Update(ExpenseReportEntity source);
        void Delete(int id);
    }
}
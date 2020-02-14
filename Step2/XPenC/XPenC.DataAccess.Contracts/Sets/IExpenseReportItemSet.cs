using System.Collections.Generic;
using XPenC.DataAccess.Contracts.Schema;

namespace XPenC.DataAccess.Contracts.Sets
{
    public interface IExpenseReportItemSet
    {
        IEnumerable<ExpenseReportItemEntity> GetAllFor(int expenseReportId);
        void AddTo(int expenseReportId, ExpenseReportItemEntity source);
        void DeleteFrom(int expenseReportId, int itemNumber);
    }
}
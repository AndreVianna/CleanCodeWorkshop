using System.Collections.Generic;
using XPenC.DataAccess.Contracts.Schema;

namespace XPenC.DataAccess.Contracts
{
    public interface IExpenseReportItemTable
    {
        IEnumerable<ExpenseReportItemEntity> GetAllFor(int expenseReportId);
        void Add(ExpenseReportItemEntity source);
        void Delete(int expenseReportId, int itemNumber);
    }
}
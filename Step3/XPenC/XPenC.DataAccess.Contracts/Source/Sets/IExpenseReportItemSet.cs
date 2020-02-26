using System.Collections.Generic;
using XPenC.BusinessLogic.Contracts.Models;

namespace XPenC.DataAccess.Contracts.Sets
{
    public interface IExpenseReportItemSet
    {
        IEnumerable<ExpenseReportItem> GetAllFor(int expenseReportId);
        void AddTo(int expenseReportId, ExpenseReportItem source);
        void DeleteFrom(int expenseReportId, int itemNumber);
    }
}
using System;
using XPenC.DataAccess.Contracts.Sets;

namespace XPenC.DataAccess.Contracts
{
    public interface IDataContext : IDisposable
    {
        IExpenseReportSet ExpenseReports { get; }
        IExpenseReportItemSet ExpenseReportItems { get; }
        void CommitChanges();
    }
}
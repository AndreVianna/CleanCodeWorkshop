using System;

namespace XPenC.DataAccess.Contracts
{
    public interface IDataContext : IDisposable
    {
        IExpenseReportSet ExpenseReports { get; }
        void CommitChanges();
    }
}
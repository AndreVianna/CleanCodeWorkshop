using XPenC.DataAccess.Contracts.Sets;

namespace XPenC.DataAccess.Contracts
{
    public interface IDataContext
    {
        IExpenseReportSet ExpenseReports { get; }
        IExpenseReportItemSet ExpenseReportItems { get; }
        void CommitChanges();
    }
}
using XPenC.DataAccess.Contracts;
using XPenC.DataAccess.Contracts.Sets;

namespace XPenC.BusinessLogic.Tests.TestDoubles
{
    internal class InMemoryDataContext : IDataContext
    {
        public InMemoryDataContext()
        {
            ExpenseReports = new InMemoryExpenseReportSet(this);
            ExpenseReportItems = new InMemoryExpenseReportItemSet();
        }

        public IExpenseReportSet ExpenseReports { get; }
        public IExpenseReportItemSet ExpenseReportItems { get; }

        public void CommitChanges() { }
    }
}
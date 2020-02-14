using XPenC.DataAccess.Contracts.Sets;

namespace XPenC.BusinessLogic.Tests.TestDoubles
{
    public class InMemoryDataContext : DummyDataContext
    {
        public InMemoryDataContext()
        {
            ExpenseReports = new InMemoryExpenseReportSet(this);
            ExpenseReportItems = new InMemoryExpenseReportItemSet();
        }

        public override IExpenseReportSet ExpenseReports { get; }
        public override IExpenseReportItemSet ExpenseReportItems { get; }
    }
}
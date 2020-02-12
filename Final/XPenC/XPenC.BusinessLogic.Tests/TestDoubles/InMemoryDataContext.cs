using XPenC.DataAccess.Contracts;

namespace XPenC.BusinessLogic.Tests
{
    public class InMemoryDataContext : DummyDataContext
    {
        public InMemoryDataContext()
        {
            ExpenseReports = new InMemoryExpenseReportSet();
        }

        public override IExpenseReportSet ExpenseReports { get; }
    }
}
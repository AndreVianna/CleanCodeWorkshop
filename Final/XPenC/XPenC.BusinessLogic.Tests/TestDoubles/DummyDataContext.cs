using XPenC.DataAccess.Contracts;

namespace XPenC.BusinessLogic.Tests
{
    public class DummyDataContext : IDataContext
    {
        public virtual IExpenseReportSet ExpenseReports => throw new System.NotImplementedException();
        public virtual void CommitChanges() => throw new System.NotImplementedException();
        public virtual void Dispose() => throw new System.NotImplementedException();
    }
}
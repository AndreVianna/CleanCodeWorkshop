using XPenC.DataAccess.Contracts;
using XPenC.DataAccess.Contracts.Sets;

namespace XPenC.BusinessLogic.Tests.TestDoubles
{
    internal class DummyDataContext : IDataContext
    {
        public virtual IExpenseReportSet ExpenseReports => throw new System.NotImplementedException();
        public virtual IExpenseReportItemSet ExpenseReportItems => throw new System.NotImplementedException();
        public virtual void CommitChanges() => throw new System.NotImplementedException();
        public virtual void Dispose() => throw new System.NotImplementedException();
    }
}
using XPenC.DataAccess.Contracts;
using XPenC.DataAccess.Contracts.Sets;

namespace XPenC.WebApp.Controllers.Tests.TestDoubles
{
    internal class DummyDataContext : IDataContext
    {
        public IExpenseReportSet ExpenseReports => throw new System.NotImplementedException();

        public IExpenseReportItemSet ExpenseReportItems => throw new System.NotImplementedException();

        public virtual void CommitChanges() => throw new System.NotImplementedException();
        public void Dispose() => throw new System.NotImplementedException();
    }
}
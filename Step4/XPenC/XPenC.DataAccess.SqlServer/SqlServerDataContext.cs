using XPenC.DataAccess.Contracts;
using XPenC.DataAccess.Contracts.Sets;
using XPenC.DataAccess.SqlServer.Sets;

namespace XPenC.DataAccess.SqlServer
{
    public sealed class SqlServerDataContext : IDataContext
    {
        private readonly ISqlDataProvider _sqlDataProvider;

        public SqlServerDataContext(ISqlDataProvider sqlDataProvider)
        {
            _sqlDataProvider = sqlDataProvider;
            ExpenseReportItems = new ExpenseReportItemSet(_sqlDataProvider);
            ExpenseReports = new ExpenseReportSet(_sqlDataProvider, ExpenseReportItems);
        }

        public IExpenseReportSet ExpenseReports { get; }

        public IExpenseReportItemSet ExpenseReportItems { get; }

        public void CommitChanges()
        {
            _sqlDataProvider.CommitChanges();
        }
    }
}
using Microsoft.Extensions.Configuration;
using XPenC.DataAccess.Contracts;
using XPenC.DataAccess.Contracts.Sets;
using XPenC.DataAccess.SqlServer.Sets;

namespace XPenC.DataAccess.SqlServer
{
    public sealed class SqlServerDataContext : IDataContext
    {
        private readonly SqlConnectionHandler _sqlConnection;

        public SqlServerDataContext(IConfiguration configuration, string connectionStringName)
        {
            _sqlConnection = new SqlConnectionHandler(configuration, connectionStringName);
            ExpenseReportItems = new ExpenseReportItemSet(_sqlConnection);
            ExpenseReports = new ExpenseReportSet(_sqlConnection, ExpenseReportItems);
        }

        private bool _isDisposed;
        public void Dispose()
        {
            if (_isDisposed)
            {
                return;
            }

            _sqlConnection?.Dispose();
            _isDisposed = true;
        }

        public IExpenseReportSet ExpenseReports { get; }

        public IExpenseReportItemSet ExpenseReportItems { get; }

        public void CommitChanges()
        {
            _sqlConnection.CommitChanges();
        }
    }
}
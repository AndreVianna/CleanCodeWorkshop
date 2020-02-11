using XPenC.WebApp.BusinessLogic;

namespace XPenC.WebApp.DataAccess
{
    public sealed class DataContext : IDataContext
    {
        private readonly IConnectionHandler _connection;

        public DataContext(IConnectionHandler connection)
        {
            _connection = connection;
            ExpenseReportItems = new ExpenseReportItemTable(connection);
            ExpenseReports = new ExpenseReportTable(connection, ExpenseReportItems);
        }

        private bool _isDisposed;
        public void Dispose()
        {
            if (_isDisposed) return;
            _connection?.Dispose();
            _isDisposed = true;
        }

        public IExpenseReportTable ExpenseReports { get; }

        public IExpenseReportItemTable ExpenseReportItems { get; }

        public void CommitChanges()
        {
            _connection.CommitChanges();
        }
    }
}
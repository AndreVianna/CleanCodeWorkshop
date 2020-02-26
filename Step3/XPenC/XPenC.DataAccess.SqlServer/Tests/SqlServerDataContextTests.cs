using XPenC.DataAccess.SqlServer.Tests.TestDoubles;
using Xunit;

namespace XPenC.DataAccess.SqlServer.Tests
{
    public class SqlServerDataContextTests
    {
        private readonly SqlServerDataContext _context;

        public SqlServerDataContextTests()
        {
            _context = new SqlServerDataContext(new MockSqlDataProvider());
        }

        [Fact]
        public void SqlServerDataContext_Sets_ShouldPass()
        {
            Assert.NotNull(_context.ExpenseReports);
            Assert.NotNull(_context.ExpenseReportItems);
        }

        [Fact]
        public void SqlServerDataContext_CommitChanges_ShouldPass()
        {
            _context.CommitChanges();
        }
    }
}
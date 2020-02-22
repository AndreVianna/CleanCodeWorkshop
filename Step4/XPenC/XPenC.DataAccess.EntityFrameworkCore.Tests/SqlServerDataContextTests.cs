using Xunit;

namespace XPenC.DataAccess.EntityFrameworkCore.Tests
{
    public class EntityFrameworkDataContextTests
    {
        private readonly EntityFrameworkDataContext _context;

        public EntityFrameworkDataContextTests()
        {
            var dbContext = new XPenCDbContext(InMemoryDbContextOptionsBuilder<XPenCDbContext>.Build());
            _context = new EntityFrameworkDataContext(dbContext);
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
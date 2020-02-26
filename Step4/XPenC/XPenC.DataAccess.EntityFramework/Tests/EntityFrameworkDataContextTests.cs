using XPenC.BusinessLogic.Contracts.Models;
using Xunit;

namespace XPenC.DataAccess.EntityFramework.Tests
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

        [Fact]
        public void SqlServerDataContext_CommitChanges_WithExpenseReportAddAfterSaveAction_ShouldPass()
        {
            _context.ExpenseReports.Add(new ExpenseReport { Client = "Some Client" });
            _context.CommitChanges();
        }

        [Fact]
        public void SqlServerDataContext_CommitChanges_WithExpenseReportUpdateAfterSaveAction_ShouldPass()
        {
            var report = new ExpenseReport { Client = "Some Client" };
            _context.ExpenseReports.Add(report);
            _context.CommitChanges();

            report.Client = "Other Client"; 
            _context.ExpenseReports.Update(report);
            _context.CommitChanges();
        }

        [Fact]
        public void SqlServerDataContext_CommitChanges_WithExpenseReportItemAddToAfterSaveAction_ShouldPass()
        {
            var report = new ExpenseReport { Client = "Some Client" };
            _context.ExpenseReports.Add(report);
            _context.CommitChanges();

            var reportItem = new ExpenseReportItem { ExpenseReportId = report.Id, ExpenseType = ExpenseType.Meal, Value = 10 };
            _context.ExpenseReportItems.AddTo(report.Id, reportItem);
            _context.CommitChanges();
        }
    }
}
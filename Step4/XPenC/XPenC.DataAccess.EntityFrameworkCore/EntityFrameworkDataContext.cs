using XPenC.DataAccess.Contracts;
using XPenC.DataAccess.Contracts.Sets;
using XPenC.DataAccess.EntityFrameworkCore.Sets;

namespace XPenC.DataAccess.EntityFrameworkCore
{
    public class EntityFrameworkDataContext : IDataContext
    {
        private readonly XPenCDbContext _dbContext;

        public EntityFrameworkDataContext(XPenCDbContext dbContext)
        {
            _dbContext = dbContext;
            ExpenseReports = new ExpenseReportSet(_dbContext);
            ExpenseReportItems = new ExpenseReportItemSet(_dbContext);
        }

        public IExpenseReportSet ExpenseReports { get; }

        public IExpenseReportItemSet ExpenseReportItems { get; }

        public void CommitChanges() => _dbContext.SaveChanges();
    }
}

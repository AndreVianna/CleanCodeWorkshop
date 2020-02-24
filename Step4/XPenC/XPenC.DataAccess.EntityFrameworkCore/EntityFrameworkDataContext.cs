using System;
using System.Collections.Generic;
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
            ExpenseReports = new ExpenseReportSet(_dbContext, AfterSaveChanges);
            ExpenseReportItems = new ExpenseReportItemSet(_dbContext, AfterSaveChanges);
        }

        public IExpenseReportSet ExpenseReports { get; }

        public IExpenseReportItemSet ExpenseReportItems { get; }

        public ICollection<Action> AfterSaveChanges { get; } = new List<Action>();

        public void CommitChanges()
        {
            _dbContext.SaveChanges();
            foreach (var execute in AfterSaveChanges)
            {
                execute();
            }
        }
    }
}

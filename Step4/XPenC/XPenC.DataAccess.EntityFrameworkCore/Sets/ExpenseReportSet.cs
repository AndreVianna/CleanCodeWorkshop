using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using XPenC.DataAccess.Contracts.Schema;
using XPenC.DataAccess.Contracts.Sets;

namespace XPenC.DataAccess.EntityFrameworkCore.Sets
{
    public class ExpenseReportSet : IExpenseReportSet
    {
        private readonly XPenCDbContext _dataContext;

        public ExpenseReportSet(XPenCDbContext dataContext)
        {
            _dataContext = dataContext;
        }

        public void Add(ExpenseReportEntity source)
        {
            _dataContext.ExpenseReports.Add(source);
        }

        public IEnumerable<ExpenseReportEntity> GetAll()
        {
            return _dataContext.ExpenseReports.AsNoTracking().OrderByDescending(i => i.ModifiedOn);
        }

        public ExpenseReportEntity Find(int id)
        {
            return _dataContext.ExpenseReports.Include(i => i.Items).AsNoTracking().FirstOrDefault(i => i.Id == id);
        }

        public void Update(ExpenseReportEntity source)
        {
            _dataContext.ExpenseReports.Update(source);
        }

        public void Delete(int id)
        {
            var entity = _dataContext.ExpenseReports.Find(id);
            if (entity != null)
            {
                _dataContext.ExpenseReports.Remove(entity);
            }
        }
    }
}
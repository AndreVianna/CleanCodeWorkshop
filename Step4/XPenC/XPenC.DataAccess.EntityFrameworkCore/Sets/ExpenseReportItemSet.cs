using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using XPenC.DataAccess.Contracts.Schema;
using XPenC.DataAccess.Contracts.Sets;

namespace XPenC.DataAccess.EntityFrameworkCore.Sets
{
    public class ExpenseReportItemSet : IExpenseReportItemSet
    {
        private readonly XPenCDbContext _dataContext;

        public ExpenseReportItemSet(XPenCDbContext dataContext)
        {
            _dataContext = dataContext;
        }

        public IEnumerable<ExpenseReportItemEntity> GetAllFor(int expenseReportId)
        {
            return _dataContext.ExpenseReportItems.AsNoTracking().Where(i => i.ExpenseReportId == expenseReportId).ToArray();
        }

        public void AddTo(int expenseReportId, ExpenseReportItemEntity source)
        {
            source.ItemNumber = GetNextNumber(expenseReportId);
            _dataContext.ExpenseReportItems.Add(source);
        }

        public void DeleteFrom(int expenseReportId, int itemNumber)
        {
            var entity = _dataContext.ExpenseReportItems.Find(expenseReportId, itemNumber);
            if (entity != null)
            {
                _dataContext.ExpenseReportItems.Remove(entity);
            }
        }

        private int GetNextNumber(int id)
        {
            var lastNumber = _dataContext.ExpenseReportItems.AsNoTracking().Where(i => i.ExpenseReportId == id).OrderByDescending(i => i.ItemNumber).Select(i => i.ItemNumber).FirstOrDefault();
            return lastNumber + 1;
        }
    }
}
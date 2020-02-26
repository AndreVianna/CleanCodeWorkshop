using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using XPenC.BusinessLogic.Contracts.Models;
using XPenC.DataAccess.Contracts.Sets;
using static XPenC.DataAccess.SqlServer.EntityFramework.ConversionHelper;

namespace XPenC.DataAccess.SqlServer.EntityFramework.Sets
{
    public class ExpenseReportItemSet : IExpenseReportItemSet
    {
        private readonly XPenCDbContext _dbContext;
        private readonly ICollection<Action> _afterSaveChanges;

        public ExpenseReportItemSet(XPenCDbContext dbContext, ICollection<Action> afterSaveChanges)
        {
            _dbContext = dbContext;
            _afterSaveChanges = afterSaveChanges;
        }

        public IEnumerable<ExpenseReportItem> GetAllFor(int expenseReportId)
        {
            var result = _dbContext.ExpenseReportItems
                .AsNoTracking()
                .Where(i => i.ExpenseReportId == expenseReportId)
                .AsEnumerable()
                .Select(ToExpenseReportItem)
                .ToArray();
            return result;
        }

        public void AddTo(int expenseReportId, ExpenseReportItem source)
        {
            var entity = ToExpenseReportItemEntity(source);
            entity.ItemNumber = GetNextNumber(expenseReportId);
            _dbContext.ExpenseReportItems.Add(entity);
            _afterSaveChanges.Add(() => UpdateExpenseReportItem(source, entity));
        }

        public void DeleteFrom(int expenseReportId, int itemNumber)
        {
            var entity = _dbContext.ExpenseReportItems.Find(expenseReportId, itemNumber);
            if (entity != null)
            {
                _dbContext.ExpenseReportItems.Remove(entity);
            }
        }

        private int GetNextNumber(int id)
        {
            var lastNumber = _dbContext.ExpenseReportItems.AsNoTracking().Where(i => i.ExpenseReportId == id).OrderByDescending(i => i.ItemNumber).Select(i => i.ItemNumber).FirstOrDefault();
            return lastNumber + 1;
        }
    }
}
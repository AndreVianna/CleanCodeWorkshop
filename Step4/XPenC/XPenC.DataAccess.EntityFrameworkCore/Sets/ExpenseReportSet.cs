using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using XPenC.BusinessLogic.Contracts.Models;
using XPenC.DataAccess.Contracts.Sets;
using static XPenC.DataAccess.EntityFrameworkCore.ConversionHelper;

namespace XPenC.DataAccess.EntityFrameworkCore.Sets
{
    public class ExpenseReportSet : IExpenseReportSet
    {
        private readonly XPenCDbContext _dbContext;
        private readonly ICollection<Action> _afterSaveChanges;

        public ExpenseReportSet(XPenCDbContext dbContext, ICollection<Action> afterSaveChanges)
        {
            _dbContext = dbContext;
            _afterSaveChanges = afterSaveChanges;
        }

        public void Add(ExpenseReport source)
        {
            var entity = ToExpenseReportEntity(source);
            _dbContext.ExpenseReports.Add(entity);
            _afterSaveChanges.Add(() => UpdateExpenseReport(source, entity));
        }

        public IEnumerable<ExpenseReport> GetAll()
        {
            var result = _dbContext.ExpenseReports
                .AsNoTracking()
                .OrderByDescending(i => i.ModifiedOn)
                .Select(ToExpenseReport)
                .ToArray();
            return result;
        }

        public ExpenseReport Find(int id)
        {
            var result = _dbContext.ExpenseReports
                .Include(i => i.Items)
                .AsNoTracking()
                .FirstOrDefault(i => i.Id == id);
            return ToExpenseReport(result);
        }

        public void Update(ExpenseReport source)
        {
            var entity = _dbContext.ExpenseReports.Find(source.Id);
            UpdateExpenseReportEntity(entity, source);
            var itemsNumber = source.Items.Select(i => i.ItemNumber);
            var missingItems = _dbContext.ExpenseReportItems.Where(i => i.ExpenseReportId == source.Id && !itemsNumber.Contains(i.ItemNumber));
            _dbContext.ExpenseReportItems.RemoveRange(missingItems);
            _dbContext.ExpenseReports.Update(entity);
            _afterSaveChanges.Add(() => UpdateExpenseReport(source, entity));
        }

        public void Delete(int id)
        {
            var entity = _dbContext.ExpenseReports.Find(id);
            if (entity != null)
            {
                _dbContext.ExpenseReports.Remove(entity);
            }
        }
    }
}
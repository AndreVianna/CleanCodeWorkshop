using System;
using System.Collections.Generic;
using System.Linq;
using XPenC.BusinessLogic.Contracts;
using XPenC.BusinessLogic.Contracts.Models;
using XPenC.DataAccess.Contracts;

namespace XPenC.BusinessLogic
{
    public class ExpenseReportOperations : IExpenseReportOperations
    {
        private readonly IDataContext _dataContext;

        public ExpenseReportOperations(IDataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public ExpenseReport CreateWithDefaults()
        {
            var now = DateTime.Now;
            return new ExpenseReport
            {
                CreatedOn = now,
                ModifiedOn = now,
            };
        }

        public void Add(ExpenseReport source)
        {
            _dataContext.ExpenseReports.Add(ConversionHelper.ToExpenseReportEntity(source));
        }

        public List<ExpenseReport> GetList()
        {
            return _dataContext.ExpenseReports.GetAll().Select(ConversionHelper.ToExpenseReport).ToList();
        }

        public ExpenseReport Find(int id)
        {
            return ConversionHelper.ToExpenseReport(_dataContext.ExpenseReports.Find(id));
        }

        public void RemoveItem(ExpenseReport source, int itemNumber)
        {
            var itemToRemove = source.Items.ToList().Find(i => i.ItemNumber == itemNumber);
            source.Items.Remove(itemToRemove);
            _dataContext.ExpenseReports.Update(ConversionHelper.ToExpenseReportEntity(source));
        }

        public void AddItem(ExpenseReport source, ExpenseReportItem newItem)
        {
            if (source.Items == null)
                source.Items = new List<ExpenseReportItem>();
            source.Items.Add(new ExpenseReportItem
            {
                ExpenseReportId = source.Id,
                Date = newItem.Date,
                ExpenseType = newItem.ExpenseType,
                Value = newItem.Value,
            });
            _dataContext.ExpenseReports.Update(ConversionHelper.ToExpenseReportEntity(source));
        }

        public void Update(ExpenseReport source)
        {
            _dataContext.ExpenseReports.Update(ConversionHelper.ToExpenseReportEntity(source));
        }

        public void UpdateLastModificationDate(ExpenseReport source)
        {
            source.ModifiedOn = DateTime.Now;
            _dataContext.ExpenseReports.Update(ConversionHelper.ToExpenseReportEntity(source));
        }

        public void Delete(int id)
        {
            _dataContext.ExpenseReports.Delete(id);
        }
    }
}
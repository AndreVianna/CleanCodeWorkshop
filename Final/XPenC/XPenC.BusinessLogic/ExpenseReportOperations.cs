using System;
using System.Collections.Generic;
using System.Linq;
using XPenC.BusinessLogic.Contracts;
using XPenC.BusinessLogic.Contracts.Models;
using XPenC.DataAccess.Contracts;
using static XPenC.BusinessLogic.ConversionHelper;

namespace XPenC.BusinessLogic
{
    public class ExpenseReportOperations : IExpenseReportOperations
    {
        private readonly IDataContext _dataContext;
        private static decimal _maximumMealValue = 50m;

        public ExpenseReportOperations(IDataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public static bool IsExpenseAboveMaximum(ExpenseReportItem item) => item.ExpenseType == ExpenseType.Meal && item.Value > _maximumMealValue;
        public static decimal CalculateReportTotal(IEnumerable<ExpenseReportItem> items) => items.Sum(i => i.Value ?? 0);
        public static decimal CalculateReportMealTotal(IEnumerable<ExpenseReportItem> items) => items.Where(i => i.ExpenseType == ExpenseType.Meal).Sum(i => i.Value ?? 0);


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
            var newEntity = ToExpenseReportEntity(source);
            _dataContext.ExpenseReports.Add(newEntity);
            UpdateExpenseReport(source, newEntity);
        }

        public IEnumerable<ExpenseReport> GetList()
        {
            return _dataContext.ExpenseReports.GetAll().Select(ToExpenseReport);
        }

        public ExpenseReport Find(int id)
        {
            return ToExpenseReport(_dataContext.ExpenseReports.Find(id));
        }

        public void RemoveItem(ExpenseReport source, int itemNumber)
        {
            _dataContext.ExpenseReportItems.DeleteFrom(source.Id, itemNumber);
            var itemToRemove = source.Items.FirstOrDefault(i => i.ItemNumber == itemNumber);
            if (itemToRemove != null)
            {
                source.Items.Remove(itemToRemove);
            }
        }

        public void AddItem(ExpenseReport source, ExpenseReportItem newItem)
        {
            var newItemEntity = ToExpenseReportItemEntity(newItem);
            _dataContext.ExpenseReportItems.AddTo(source.Id, newItemEntity);
            UpdateExpenseReportItem(newItem, newItemEntity);
            source.Items.Add(newItem);
        }

        public void Update(ExpenseReport source)
        {
            source.ModifiedOn = DateTime.Now;
            _dataContext.ExpenseReports.Update(ToExpenseReportEntity(source));
        }

        public void Delete(int id)
        {
            _dataContext.ExpenseReports.Delete(id);
        }
    }
}
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
        private const decimal MAXIMUM_MEAL_VALUE = 50m;

        public ExpenseReportOperations(IDataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public static bool IsExpenseAboveMaximum(ExpenseReportItem item) => item.ExpenseType == ExpenseType.Meal && item.Value > MAXIMUM_MEAL_VALUE;
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
            var entity = ToExpenseReportEntity(source);
            _dataContext.ExpenseReports.Add(entity);
            source.Id = entity.Id;
        }

        public List<ExpenseReport> GetList()
        {
            return _dataContext.ExpenseReports.GetAll().Select(ToExpenseReport).ToList();
        }

        public ExpenseReport Find(int id)
        {
            return ToExpenseReport(_dataContext.ExpenseReports.Find(id));
        }

        public void RemoveItem(ExpenseReport source, int itemNumber)
        {
            var itemToRemove = source.Items.ToList().Find(i => i.ItemNumber == itemNumber);
            source.Items.Remove(itemToRemove);
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
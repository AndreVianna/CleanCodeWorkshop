using System;
using System.Collections.Generic;
using System.Linq;
using XPenC.BusinessLogic.Contracts;
using XPenC.BusinessLogic.Contracts.Models;
using XPenC.BusinessLogic.Validation;
using XPenC.DataAccess.Contracts;
using static XPenC.BusinessLogic.ConversionHelper;

namespace XPenC.BusinessLogic
{
    public class ExpenseReportOperations : IExpenseReportOperations
    {
        private readonly IDataContext _dataContext;

        public ExpenseReportOperations(IDataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public static decimal MaximumMealValue { get; } = 50m;

        public static bool IsExpenseAboveMaximum(ExpenseReportItem item) => item.ExpenseType == ExpenseType.Meal && item.Value > MaximumMealValue;
        public static decimal CalculateReportTotal(IEnumerable<ExpenseReportItem> items) => items.Sum(i => i.Value);
        public static decimal CalculateReportMealTotal(IEnumerable<ExpenseReportItem> items) => items.Where(i => i.ExpenseType == ExpenseType.Meal).Sum(i => i.Value);

        public ExpenseReport CreateWithDefaults()
        {
            var now = DateTime.Now;
            return new ExpenseReport
            {
                CreatedOn = now, 
                ModifiedOn = now
            };
        }

        public void Add(ExpenseReport source)
        {
            ValidateOperation(nameof(Add), source);
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

        public void Update(ExpenseReport source)
        {
            ValidateOperation(nameof(Update), source);
            source.ModifiedOn = DateTime.Now;
            _dataContext.ExpenseReports.Update(ToExpenseReportEntity(source));
        }

        public void Delete(int id)
        {
            _dataContext.ExpenseReports.Delete(id);
        }

        private void ValidateOperation(string operation, ExpenseReport input)
        {
            var validator = new OperationValidator(operation);
            ValidateExpenseReportClient(validator, input);
            validator.Validate();
        }

        private static void ValidateExpenseReportClient(OperationValidator validator, ExpenseReport input)
        {
            if (string.IsNullOrWhiteSpace(input.Client))
            {
                validator.AddError(nameof(ExpenseReport.Client), $"The '{nameof(ExpenseReport.Client)}' field is required.");
            }
        }
    }
}
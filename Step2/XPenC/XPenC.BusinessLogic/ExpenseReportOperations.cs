using System;
using System.Collections.Generic;
using System.Linq;
using XPenC.BusinessLogic.Contracts;
using XPenC.BusinessLogic.Contracts.Models;
using XPenC.BusinessLogic.Validation;
using XPenC.DataAccess.Contracts;
using static XPenC.BusinessLogic.ExpenseReportItemOperations;

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
                ModifiedOn = now
            };
        }

        public void Add(ExpenseReport source)
        {
            ValidateOperation(nameof(Add), source);
            ProcessRulesForUpdate(source);
            _dataContext.ExpenseReports.Add(source);
        }

        public IEnumerable<ExpenseReport> GetList()
        {
            var expenseReports = _dataContext.ExpenseReports.GetAll().ToArray();
            foreach (var expenseReport in expenseReports)
            {
                ProcessRulesForDisplay(expenseReport);
            }
            return expenseReports;
        }

        public ExpenseReport Find(int id)
        {
            var expenseReport = _dataContext.ExpenseReports.Find(id);
            ProcessRulesForDisplay(expenseReport);
            return expenseReport;
        }

        public void Update(ExpenseReport source)
        {
            ValidateOperation(nameof(Update), source);
            ProcessRulesForUpdate(source);
            _dataContext.ExpenseReports.Update(source);
        }

        public void Delete(int id)
        {
            _dataContext.ExpenseReports.Delete(id);
        }

        private static void ValidateOperation(string operation, ExpenseReport input)
        {
            var validator = new OperationValidator(operation);
            ValidateClient(validator, input);
            validator.Validate();
        }

        private static void ValidateClient(OperationValidator validator, ExpenseReport input)
        {
            if (string.IsNullOrWhiteSpace(input.Client))
            {
                validator.AddError(nameof(ExpenseReport.Client), $"The '{nameof(ExpenseReport.Client)}' field is required.");
            }
        }

        private static void ProcessRulesForDisplay(ExpenseReport expenseReport)
        {
            if (expenseReport == null)
            {
                return;
            }
            foreach (var item in expenseReport.Items)
            {
                ProcessItemRules(item);
            }
        }

        private static void ProcessRulesForUpdate(ExpenseReport expenseReport)
        {
            expenseReport.ModifiedOn = DateTime.Now;
            expenseReport.MealTotal = CalculateMealTotal(expenseReport.Items);
            expenseReport.Total = CalculateTotal(expenseReport.Items);
            foreach (var item in expenseReport.Items)
            {
                ProcessItemRules(item);
            }
        }

        private static decimal CalculateTotal(IEnumerable<ExpenseReportItem> items) => items.Sum(i => i.Value);
        private static decimal CalculateMealTotal(IEnumerable<ExpenseReportItem> items) => items.Where(i => i.ExpenseType == ExpenseType.Meal).Sum(i => i.Value);
    }
}
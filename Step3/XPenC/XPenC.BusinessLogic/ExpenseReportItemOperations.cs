using System;
using XPenC.BusinessLogic.Contracts;
using XPenC.BusinessLogic.Contracts.Models;
using XPenC.BusinessLogic.Validation;
using XPenC.DataAccess.Contracts;

namespace XPenC.BusinessLogic
{
    public class ExpenseReportItemOperations : IExpenseReportItemOperations
    {
        private readonly IDataContext _dataContext;

        public ExpenseReportItemOperations(IDataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public void Add(ExpenseReportItem newItem)
        {
            ValidateAddOperation(newItem);
            _dataContext.ExpenseReportItems.AddTo(newItem.ExpenseReportId, newItem);
        }

        public void Delete(int expenseReportId, int itemNumber)
        {
            _dataContext.ExpenseReportItems.DeleteFrom(expenseReportId, itemNumber);
        }

        private void ValidateAddOperation(ExpenseReportItem input)
        {
            var validator = new OperationValidator(nameof(Add));
            ValidateExpenseReportItemDate(validator, input);
            ValidateExpenseReportItemValue(validator, input);
            validator.Validate();
        }

        private static void ValidateExpenseReportItemDate(OperationValidator validator, ExpenseReportItem input)
        {
            if (input.Date > DateTime.Now.Date)
            {
                validator.AddError(nameof(ExpenseReportItem.Date), "The expense item date must not be in the future.");
            }
        }

        private static void ValidateExpenseReportItemValue(OperationValidator validator, ExpenseReportItem input)
        {
            if (input.Value <= 0)
            {
                validator.AddError(nameof(ExpenseReportItem.Value), "The expense item value must be grater than zero.");
            }
        }

        internal static void ProcessExpenseReportItemRules(ExpenseReportItem item)
        {
            item.IsAboveMaximum = IsExpenseAboveMaximum(item);
        }
        
        private static decimal MaximumMealValue { get; } = 50m;
        private static bool IsExpenseAboveMaximum(ExpenseReportItem item) => item.ExpenseType == ExpenseType.Meal && item.Value > MaximumMealValue;
    }
}
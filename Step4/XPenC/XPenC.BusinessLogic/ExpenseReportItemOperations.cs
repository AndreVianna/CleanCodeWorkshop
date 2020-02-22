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

        public void Add(int expenseReportId, ExpenseReportItem newItem)
        {
            ValidateAddOperation(newItem);
            var newItemEntity = ConversionHelper.ToExpenseReportItemEntity(newItem);
            _dataContext.ExpenseReportItems.AddTo(expenseReportId, newItemEntity);
            ConversionHelper.UpdateExpenseReportItem(newItem, newItemEntity);
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
    }
}
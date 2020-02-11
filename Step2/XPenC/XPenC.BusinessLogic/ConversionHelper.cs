using System.Linq;
using XPenC.BusinessLogic.Contracts.Models;
using XPenC.DataAccess.Contracts.Schema;

namespace XPenC.BusinessLogic
{
    internal static class ConversionHelper
    {
        public static ExpenseReport ToExpenseReport(ExpenseReportEntity source)
        {
            if (source == null) return null;
            return new ExpenseReport
            {
                Id = source.Id,
                Client = source.Client,
                CreatedOn = source.CreatedOn,
                ModifiedOn = source.ModifiedOn ?? source.CreatedOn,
                Items = source.Items?.Select(ToExpenseReportItem).ToList()
            };
        }

        public static ExpenseReportEntity ToExpenseReportEntity(ExpenseReport source)
        {
            if (source == null) return null;
            return new ExpenseReportEntity
            {
                Id = source.Id,
                Client = source.Client,
                CreatedOn = source.CreatedOn,
                ModifiedOn = source.ModifiedOn ?? source.CreatedOn,
                Items = (source.Items?.Select(ToExpenseReportItemEntity) ?? Enumerable.Empty<ExpenseReportItemEntity>()).ToList()
            };
        }

        public static ExpenseReportItem ToExpenseReportItem(ExpenseReportItemEntity source)
        {
            return new ExpenseReportItem
            {
                ExpenseReportId = source.ExpenseReportId,
                ItemNumber = source.ItemNumber,
                ExpenseType = source.ExpenseType,
                Date = source.Date,
                Value = source.Value,
                Description = source.Description,
            };
        }

        public static ExpenseReportItemEntity ToExpenseReportItemEntity(ExpenseReportItem source)
        {
            return new ExpenseReportItemEntity
            {
                ExpenseReportId = source.ExpenseReportId,
                ItemNumber = source.ItemNumber,
                ExpenseType = source.ExpenseType,
                Date = source.Date,
                Value = source.Value,
                Description = source.Description,
            };
        }
    }
}
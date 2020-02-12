using System.Linq;
using XPenC.BusinessLogic.Contracts;
using XPenC.BusinessLogic.Contracts.Models;
using XPenC.DataAccess.Contracts.Schema;

namespace XPenC.BusinessLogic
{
    internal static class ConversionHelper
    {
        public static ExpenseReport ToExpenseReport(ExpenseReportEntity source)
        {
            if (source == null) return null;
            var items = (source.Items?.Select(ToExpenseReportItem)?? Enumerable.Empty<ExpenseReportItem>()).ToList();
            var value = new ExpenseReport
            {
                Id = source.Id,
                Client = source.Client,
                CreatedOn = source.CreatedOn,
                ModifiedOn = source.ModifiedOn ?? source.CreatedOn,
                Items = items,
                Total = ExpenseReportOperations.CalculateReportTotal(items),
                MealTotal = ExpenseReportOperations.CalculateReportMealTotal(items),
            };
            return value;
        }

        public static ExpenseReportEntity ToExpenseReportEntity(ExpenseReport source)
        {
            if (source == null) return null;
            return new ExpenseReportEntity
            {
                Id = source.Id,
                Client = source.Client,
                CreatedOn = source.CreatedOn,
                ModifiedOn = source.ModifiedOn,
                Items = (source.Items?.Select(ToExpenseReportItemEntity) ?? Enumerable.Empty<ExpenseReportItemEntity>()).ToList(),
                Total = ExpenseReportOperations.CalculateReportTotal(source.Items),
                MealTotal = ExpenseReportOperations.CalculateReportMealTotal(source.Items),
            };
        }

        private static ExpenseReportItem ToExpenseReportItem(ExpenseReportItemEntity source)
        {
            var result = new ExpenseReportItem
            {
                ExpenseReportId = source.ExpenseReportId,
                ItemNumber = source.ItemNumber,
                ExpenseType = TranslateExpenseType(source.ExpenseType),
                Date = source.Date,
                Value = source.Value,
                Description = source.Description,
            };
            result.IsAboveMaximum = ExpenseReportOperations.IsExpenseAboveMaximum(result);
            return result;
        }

        private static ExpenseReportItemEntity ToExpenseReportItemEntity(ExpenseReportItem source)
        {
            return new ExpenseReportItemEntity
            {
                ExpenseReportId = source.ExpenseReportId,
                ItemNumber = source.ItemNumber,
                ExpenseType = TranslateExpenseType(source.ExpenseType),
                Date = source.Date,
                Value = source.Value,
                Description = source.Description,
            };
        }

        private static ExpenseType TranslateExpenseType(string expenseType)
        {
            switch (expenseType)
            {
                case "O": return ExpenseType.Office;
                case "M": return ExpenseType.Meal;
                case "L": return ExpenseType.HotelLodging;
                case "L*": return ExpenseType.OtherLodging;
                case "TL": return ExpenseType.LandTransportation;
                case "TA": return ExpenseType.AirTransportation;
                case "Ot": return ExpenseType.Other;
                default: return ExpenseType.Unknown;
            }
        }

        private static string TranslateExpenseType(ExpenseType expenseType)
        {
            switch (expenseType)
            {
                case ExpenseType.Office: return "O";
                case ExpenseType.Meal: return "M";
                case ExpenseType.HotelLodging: return "L";
                case ExpenseType.OtherLodging: return "L*";
                case ExpenseType.LandTransportation: return "TL";
                case ExpenseType.AirTransportation: return "TA";
                case ExpenseType.Other: return "Ot";
                default: return null;
            }
        }
    }
}
using System.Linq;
using XPenC.BusinessLogic.Contracts.Models;

namespace XPenC.WebApp.ViewModels
{
    internal static class ConversionHelper
    {
        public static ExpenseReportListItem ToExpenseReportListItem(ExpenseReport source)
        {
            return new ExpenseReportListItem
            {
                Id = source.Id,
                Client = source.Client,
                CreatedOn = source.CreatedOn,
                ModifiedOn = source.ModifiedOn ?? source.CreatedOn,
            };
        }

        public static ExpenseReportDetails ToExpenseReportDetails(ExpenseReport source)
        {
            return new ExpenseReportDetails
            {
                Id = source.Id,
                Client = source.Client,
                CreatedOn = source.CreatedOn,
                ModifiedOn = source.ModifiedOn ?? source.CreatedOn,
                MealTotal = source.Items.Where(i => i.ExpenseType == "M").Sum(i => i.Value ?? 0),
                Total = source.Items.Sum(i => i.Value ?? 0),
                Items = source.Items.Select(ToExpenseReportItemDetails).ToList(),
            };
        }

        public static ExpenseReportItem ToExpenseReportItem(ExpenseReportUpdateInput source)
        {
            return new ExpenseReportItem
            {
                ExpenseReportId = source.Id,
                ItemNumber = 0,
                ExpenseType = source.NewItem.ExpenseType,
                Date = source.NewItem.Date,
                Value = source.NewItem.Value,
                Description = source.NewItem.Description,
            };
        }

        private static ExpenseReportItemDetails ToExpenseReportItemDetails(ExpenseReportItem source)
        {
            return new ExpenseReportItemDetails
            {
                Number = source.ItemNumber,
                ExpenseType = TranslateExpenseType(source.ExpenseType),
                Date = source.Date,
                Description = source.Description,
                Value = source.Value ?? 0,
            };
        }

        public static ExpenseReportUpdateInput ToExpenseReportUpdateInput(ExpenseReport source)
        {
            return new ExpenseReportUpdateInput
            {
                Id = source.Id,
                Client = source.Client,
                Items = (source.Items?.Select(ToExpenseReportItemDetails) ?? Enumerable.Empty<ExpenseReportItemDetails>()).ToList(),
            };
        }

        public static string TranslateExpenseType(string expenseType)
        {
            switch (expenseType)
            {
                case "O": return "Office";
                case "M": return "Meal";
                case "L": return "Lodging (Hotel)";
                case "L*": return "Lodging (Other)";
                case "TL": return "Transportation (Land)";
                case "TA": return "Transportation (Air)";
                case "Ot": return "Other";
                default: return "Unknown";
            }
        }

        public static ExpenseReportUpdateInput RestoreInputItems(ExpenseReport source, ExpenseReportUpdateInput dest)
        {
            dest.Items = source.Items.Select(ToExpenseReportItemDetails).ToList();
            return dest;
        }
    }
}
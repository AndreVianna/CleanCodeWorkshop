using System;
using System.Linq;
using XPenC.BusinessLogic.Contracts;
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
                ModifiedOn = source.ModifiedOn,
            };
        }

        public static ExpenseReportDetails ToExpenseReportDetails(ExpenseReport source)
        {
            return new ExpenseReportDetails
            {
                Id = source.Id,
                Client = source.Client,
                CreatedOn = source.CreatedOn,
                ModifiedOn = source.ModifiedOn,
                MealTotal = source.MealTotal,
                Total = source.Total,
                Items = source.Items.Select(ToExpenseReportItemDetails).ToList(),
            };
        }

        public static ExpenseReportItem ToExpenseReportItem(ExpenseReportUpdateInput source)
        {
            return new ExpenseReportItem
            {
                ExpenseReportId = source.Id,
                ItemNumber = 0,
                ExpenseType = (ExpenseType)Enum.Parse(typeof(ExpenseType), source.NewItem.ExpenseType),
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
                ExpenseType = source.ExpenseType.ToString(),
                Date = source.Date,
                Description = source.Description,
                Value = source.Value ?? 0,
                IsAboveMaximum = source.IsAboveMaximum,
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

        public static ExpenseReportUpdateInput RestoreInputItems(ExpenseReport source, ExpenseReportUpdateInput dest)
        {
            dest.Items = source.Items.Select(ToExpenseReportItemDetails).ToList();
            return dest;
        }
    }
}
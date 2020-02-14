using System;
using System.Linq;
using System.Text.RegularExpressions;
using XPenC.BusinessLogic.Contracts.Models;
using XPenC.WebApp.Controllers.Controllers;

namespace XPenC.WebApp.Controllers.ViewModels
{
    internal static class ConversionHelper
    {
        public static ExpenseReportListItem ToExpenseReportListItem(ExpenseReport source)
        {
            var result = new ExpenseReportListItem();
            UpdateExpenseReportListItem(result, source);
            return result;
        }

        private static void UpdateExpenseReportListItem(ExpenseReportListItem dest, ExpenseReport source)
        {
            dest.Id = source.Id;
            dest.Client = source.Client;
            dest.CreatedOn = source.CreatedOn;
            dest.ModifiedOn = source.ModifiedOn;
        }

        public static ExpenseReportDetails ToExpenseReportDetails(ExpenseReport source)
        {
            var result = new ExpenseReportDetails();
            UpdateExpenseReportDetails(result, source);
            return result;
        }

        private static void UpdateExpenseReportDetails(ExpenseReportDetails dest, ExpenseReport source)
        {
            dest.Id = source.Id;
            dest.Client = source.Client;
            dest.CreatedOn = source.CreatedOn;
            dest.ModifiedOn = source.ModifiedOn;
            dest.MealTotal = source.MealTotal;
            dest.Total = source.Total;
            dest.Items = source.Items.Select(ToExpenseReportItemDetails).ToList();
        }

        private static ExpenseReportItemDetails ToExpenseReportItemDetails(ExpenseReportItem source)
        {
            var result = new ExpenseReportItemDetails();
            UpdateExpenseReportItemDetails(result, source);
            return result;
        }
        
        private static void UpdateExpenseReportItemDetails(ExpenseReportItemDetails dest, ExpenseReportItem source)
        {
            dest.Number = source.ItemNumber;
            dest.ExpenseType = ExpenseReportsController.GetExpenseTypeDisplayName(source.ExpenseType);
            dest.Date = source.Date;
            dest.Description = source.Description;
            dest.Value = source.Value;
            dest.IsAboveMaximum = source.IsAboveMaximum;
        }

        public static ExpenseReportItem ToExpenseReportItem(ExpenseReportUpdate source)
        {
            var result = new ExpenseReportItem();
            UpdateExpenseReportItem(result, source);
            return result;
        }

        private static void UpdateExpenseReportItem(ExpenseReportItem dest, ExpenseReportUpdate source)
        {
            dest.ExpenseReportId = source.Id;
            dest.ItemNumber = 0;
            dest.ExpenseType = (ExpenseType)Enum.Parse(typeof(ExpenseType), source.NewItem.ExpenseType);
            dest.Date = source.NewItem.Date;
            dest.Value = source.NewItem.Value;
            dest.Description = source.NewItem.Description;
        }

        public static ExpenseReportUpdate ToExpenseReportUpdate(ExpenseReport source)
        {
            var result = new ExpenseReportUpdate();
            UpdateExpenseReportUpdate(result, source);
            return result;
        }

        public static void UpdateExpenseReportUpdate(ExpenseReportUpdate dest, ExpenseReport source)
        {
            dest.Id = source.Id;
            dest.Client = source.Client;
            dest.DisplayItems = source.Items.Select(ToExpenseReportItemDisplay).ToList();
            dest.AddActionName = ExpenseReportsController.AddActionName;
            dest.FinishActionName = ExpenseReportsController.FinishActionName;
            dest.SaveActionName = ExpenseReportsController.SaveActionName;
        }

        private static ExpenseReportItemDisplay ToExpenseReportItemDisplay(ExpenseReportItem source)
        {
            var result = new ExpenseReportItemDisplay();
            UpdateExpenseReportItemDisplay(result, source);
            return result;
        }

        private static void UpdateExpenseReportItemDisplay(ExpenseReportItemDisplay dest, ExpenseReportItem source)
        {
            dest.Number = source.ItemNumber;
            dest.ExpenseType = ExpenseReportsController.GetExpenseTypeDisplayName(source.ExpenseType);
            dest.Date = source.Date;
            dest.Description = source.Description;
            dest.Value = source.Value;
            dest.IsAboveMaximum = source.IsAboveMaximum;
            dest.RemoveActionName = GenerateRemoveActionName(source);
        }

        public static bool IsRemoveAction(string action)
        {
            return Regex.IsMatch(action, GetRemoveActionPattern());
        }

        public static int GetItemNumberFromAction(string action)
        {
            return int.Parse(Regex.Match(action, GetRemoveActionPattern()).Groups[1].Value);
        }

        private static string GetRemoveActionPattern()
        {
            return $@"^{ExpenseReportsController.RemoveActionName}(\d*)$";
        }

        private static string GenerateRemoveActionName(ExpenseReportItem source)
        {
            return $"{ExpenseReportsController.RemoveActionName}{source.ItemNumber}";
        }
    }
}
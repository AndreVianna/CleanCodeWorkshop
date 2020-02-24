using System.Linq;
using XPenC.BusinessLogic.Contracts.Models;
using BusinessExpenseType = XPenC.BusinessLogic.Contracts.Models.ExpenseType;
using ViewExpenseType = XPenC.WebApp.Models.ExpenseReports.ExpenseType;

namespace XPenC.WebApp.Models.ExpenseReports
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
            dest.ExpenseType = GetViewExpenseType(source.ExpenseType);
            dest.Date = source.Date;
            dest.Description = source.Description;
            dest.Value = source.Value;
            dest.IsAboveMaximum = source.IsAboveMaximum;
        }

        public static void UpdateExpenseReport(ExpenseReport dest, ExpenseReportUpdate source)
        {
            dest.Client = source.Client;
        }

        public static ExpenseReportItem ToExpenseReportItem(ExpenseReportItemUpdate source)
        {
            var result = new ExpenseReportItem();
            UpdateExpenseReportItem(result, source);
            return result;
        }

        private static void UpdateExpenseReportItem(ExpenseReportItem dest, ExpenseReportItemUpdate source)
        {
            dest.ExpenseReportId = source.ExpenseReportId;
            dest.ItemNumber = 0;
            dest.ExpenseType = GetExpenseType(source.ExpenseType);
            dest.Date = source.Date;
            dest.Value = source.Value;
            dest.Description = source.Description;
        }

        public static ExpenseReportItemUpdate ToExpenseReportItemUpdate(ExpenseReport source)
        {
            var result = new ExpenseReportItemUpdate();
            UpdateExpenseReportItemUpdate(result, source);
            return result;
        }

        public static void UpdateExpenseReportItemUpdate(ExpenseReportItemUpdate dest, ExpenseReport source)
        {
            dest.ExpenseReportId = source.Id;
            dest.ExpenseReport = ToExpenseReportDetails(source);
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
            dest.Items = source.Items.Select(ToExpenseReportItemDetails).ToList();
        }

        private static ViewExpenseType GetViewExpenseType(BusinessExpenseType expenseType)
        {
            return expenseType switch
            {
                BusinessExpenseType.Office => ViewExpenseType.Office,
                BusinessExpenseType.Meal => ViewExpenseType.Meal,
                BusinessExpenseType.HotelLodging => ViewExpenseType.HotelLodging,
                BusinessExpenseType.OtherLodging => ViewExpenseType.OtherLodging,
                BusinessExpenseType.LandTransportation => ViewExpenseType.LandTransportation,
                BusinessExpenseType.AirTransportation => ViewExpenseType.AirTransportation,
                _ => ViewExpenseType.Other  //BusinessExpenseType.Other
            };
        }

        private static BusinessExpenseType GetExpenseType(ViewExpenseType? expenseType)
        {
            return expenseType switch
            {
                ViewExpenseType.Office => BusinessExpenseType.Office,
                ViewExpenseType.Meal => BusinessExpenseType.Meal,
                ViewExpenseType.HotelLodging => BusinessExpenseType.HotelLodging,
                ViewExpenseType.OtherLodging => BusinessExpenseType.OtherLodging,
                ViewExpenseType.LandTransportation => BusinessExpenseType.LandTransportation,
                ViewExpenseType.AirTransportation => BusinessExpenseType.AirTransportation,
                _ => BusinessExpenseType.Other  //ViewExpenseType.Other
            };
        }
    }
}
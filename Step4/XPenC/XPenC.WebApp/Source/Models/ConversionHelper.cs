using System.Linq;
using XPenC.WebApp.Models.ExpenseReports;
using BusinessExpenseReport = XPenC.BusinessLogic.Contracts.Models.ExpenseReport;
using BusinessExpenseReportItem = XPenC.BusinessLogic.Contracts.Models.ExpenseReportItem;
using BusinessExpenseType = XPenC.BusinessLogic.Contracts.Models.ExpenseType;
using ItemDetails = XPenC.WebApp.Models.ExpenseReportItems.Details;
using ItemExpenseType = XPenC.WebApp.Models.ExpenseReportItems.ExpenseType;
using ItemUpdate = XPenC.WebApp.Models.ExpenseReportItems.Update;

namespace XPenC.WebApp.Models
{
    internal static class ConversionHelper
    {
        public static ListItem ToListItem(BusinessExpenseReport source)
        {
            var result = new ListItem();
            UpdateListItem(result, source);
            return result;
        }

        private static void UpdateListItem(ListItem dest, BusinessExpenseReport source)
        {
            dest.Id = source.Id;
            dest.Client = source.Client;
            dest.CreatedOn = source.CreatedOn;
            dest.ModifiedOn = source.ModifiedOn;
        }

        public static Details ToDetails(BusinessExpenseReport source)
        {
            var result = new Details();
            UpdateDetails(result, source);
            return result;
        }

        private static void UpdateDetails(Details dest, BusinessExpenseReport source)
        {
            dest.Id = source.Id;
            dest.Client = source.Client;
            dest.CreatedOn = source.CreatedOn;
            dest.ModifiedOn = source.ModifiedOn;
            dest.MealTotal = source.MealTotal;
            dest.Total = source.Total;
            dest.Items = source.Items.Select(ToItemDetails).ToList();
        }

        private static ItemDetails ToItemDetails(BusinessExpenseReportItem source)
        {
            var result = new ItemDetails();
            UpdateItemDetails(result, source);
            return result;
        }
        
        private static void UpdateItemDetails(ItemDetails dest, BusinessExpenseReportItem source)
        {
            dest.Number = source.ItemNumber;
            dest.ExpenseType = GetItemExpenseType(source.ExpenseType);
            dest.Date = source.Date;
            dest.Description = source.Description;
            dest.Value = source.Value;
            dest.IsAboveMaximum = source.IsAboveMaximum;
        }

        public static void UpdateExpenseReport(BusinessExpenseReport dest, Update source)
        {
            dest.Client = source.Client;
        }

        public static BusinessExpenseReportItem ToExpenseReportItem(ItemUpdate source)
        {
            var result = new BusinessExpenseReportItem();
            UpdateExpenseReportItem(result, source);
            return result;
        }

        private static void UpdateExpenseReportItem(BusinessExpenseReportItem dest, ItemUpdate source)
        {
            dest.ExpenseReportId = source.ExpenseReportId;
            dest.ItemNumber = 0;
            dest.ExpenseType = GetExpenseType(source.ExpenseType);
            dest.Date = source.Date;
            dest.Value = source.Value;
            dest.Description = source.Description;
        }

        public static ItemUpdate ToItemUpdate(BusinessExpenseReport source)
        {
            var result = new ItemUpdate();
            UpdateItemUpdate(result, source);
            return result;
        }

        public static void UpdateItemUpdate(ItemUpdate dest, BusinessExpenseReport source)
        {
            dest.ExpenseReportId = source.Id;
            dest.ExpenseReport = ToDetails(source);
        }

        public static Update ToUpdate(BusinessExpenseReport source)
        {
            var result = new Update();
            UpdateUpdate(result, source);
            return result;
        }

        public static void UpdateUpdate(Update dest, BusinessExpenseReport source)
        {
            dest.Id = source.Id;
            dest.Client = source.Client;
            dest.Items = source.Items.Select(ToItemDetails).ToList();
        }

        private static ItemExpenseType GetItemExpenseType(BusinessExpenseType expenseType)
        {
            return expenseType switch
            {
                BusinessExpenseType.Office => ItemExpenseType.Office,
                BusinessExpenseType.Meal => ItemExpenseType.Meal,
                BusinessExpenseType.HotelLodging => ItemExpenseType.HotelLodging,
                BusinessExpenseType.OtherLodging => ItemExpenseType.OtherLodging,
                BusinessExpenseType.LandTransportation => ItemExpenseType.LandTransportation,
                BusinessExpenseType.AirTransportation => ItemExpenseType.AirTransportation,
                _ => ItemExpenseType.Other  //BusinessExpenseType.Other
            };
        }

        private static BusinessExpenseType GetExpenseType(ItemExpenseType? expenseType)
        {
            return expenseType switch
            {
                ItemExpenseType.Office => BusinessExpenseType.Office,
                ItemExpenseType.Meal => BusinessExpenseType.Meal,
                ItemExpenseType.HotelLodging => BusinessExpenseType.HotelLodging,
                ItemExpenseType.OtherLodging => BusinessExpenseType.OtherLodging,
                ItemExpenseType.LandTransportation => BusinessExpenseType.LandTransportation,
                ItemExpenseType.AirTransportation => BusinessExpenseType.AirTransportation,
                _ => BusinessExpenseType.Other  //ViewExpenseType.Other
            };
        }
    }
}
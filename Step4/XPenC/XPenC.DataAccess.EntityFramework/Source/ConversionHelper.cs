using System.Linq;
using XPenC.BusinessLogic.Contracts.Models;
using XPenC.DataAccess.Contracts.Exceptions;
using XPenC.DataAccess.EntityFramework.Schema;

namespace XPenC.DataAccess.EntityFramework
{
    internal static class ConversionHelper
    {
        public static ExpenseReport ToExpenseReport(ExpenseReportEntity source)
        {
            if (source == null)
            {
                return null;
            }

            var value = new ExpenseReport();
            UpdateExpenseReport(value, source);
            return value;
        }

        public static void UpdateExpenseReport(ExpenseReport dest, ExpenseReportEntity source)
        {
            dest.Id = source.Id;
            dest.Client = source.Client;
            dest.CreatedOn = source.CreatedOn;
            dest.ModifiedOn = source.ModifiedOn;
            dest.Items = source.Items.Select(ToExpenseReportItem).ToHashSet();
            dest.Total = source.Total;
            dest.MealTotal = source.MealTotal;
        }

        public static ExpenseReportItem ToExpenseReportItem(ExpenseReportItemEntity source)
        {
            var value = new ExpenseReportItem();
            UpdateExpenseReportItem(value, source);
            return value;
        }

        public static void UpdateExpenseReportItem(ExpenseReportItem dest, ExpenseReportItemEntity source)
        {
            dest.ExpenseReportId = source.ExpenseReportId;
            dest.ItemNumber = source.ItemNumber;
            dest.ExpenseType = TranslateExpenseType(source.ExpenseType);
            dest.Date = source.Date;
            dest.Value = source.Value;
            dest.Description = source.Description;
        }


        public static ExpenseReportEntity ToExpenseReportEntity(ExpenseReport source)
        {
            var value = new ExpenseReportEntity();
            UpdateExpenseReportEntity(value, source);
            return value;
        }

        public static void UpdateExpenseReportEntity(ExpenseReportEntity dest, ExpenseReport source)
        {
            dest.Id = source.Id;
            dest.Client = source.Client;
            dest.CreatedOn = source.CreatedOn;
            dest.ModifiedOn = source.ModifiedOn;
            dest.Items = source.Items.Select(ToExpenseReportItemEntity).ToHashSet();
            dest.Total = source.Total;
            dest.MealTotal = source.MealTotal;
        }

        public static ExpenseReportItemEntity ToExpenseReportItemEntity(ExpenseReportItem source)
        {
            var value = new ExpenseReportItemEntity();
            UpdateExpenseReportItemEntity(value, source);
            return value;
        }

        private static void UpdateExpenseReportItemEntity(ExpenseReportItemEntity dest, ExpenseReportItem source)
        {
            dest.ExpenseReportId = source.ExpenseReportId;
            dest.ItemNumber = source.ItemNumber;
            dest.ExpenseType = TranslateExpenseType(source.ExpenseType);
            dest.Date = source.Date;
            dest.Value = source.Value;
            dest.Description = source.Description;
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
                default: throw new DataProviderException("Invalid record expense type.");
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
                default: return "Ot";
            }
        }
    }
}
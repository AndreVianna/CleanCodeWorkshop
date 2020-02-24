using System.Data;
using XPenC.BusinessLogic.Contracts.Models;
using XPenC.DataAccess.Contracts.Exceptions;

namespace XPenC.DataAccess.SqlServer
{
    internal static class ConversionHelper
    {
        public static ExpenseReport ToExpenseReport(IDataRecord row)
        {
            var value = new ExpenseReport();
            UpdateExpenseReport(value, row);
            return value;
        }

        public static void UpdateExpenseReport(ExpenseReport record, IDataRecord row)
        {
            record.Id = row.GetInt32(row.GetOrdinal("Id"));
            record.Client = row.IsDBNull(row.GetOrdinal("Client")) ? null : row.GetString(row.GetOrdinal("Client"));
            record.CreatedOn = row.GetDateTime(row.GetOrdinal("CreatedOn"));
            record.ModifiedOn = row.GetDateTime(row.GetOrdinal("ModifiedOn"));
            record.MealTotal = row.GetDecimal(row.GetOrdinal("MealTotal"));
            record.Total = row.GetDecimal(row.GetOrdinal("Total"));
            if (row.FieldCount == 6 || row.IsDBNull(row.GetOrdinal("ExpenseReportId")))
            {
                return;
            }

            record.Items.Add(ToExpenseReportItem(row));
        }

        public static ExpenseReportItem ToExpenseReportItem(IDataRecord row)
        {
            var value = new ExpenseReportItem();
            UpdateExpenseReportItem(value, row);
            return value;
        }

        private static void UpdateExpenseReportItem(ExpenseReportItem record, IDataRecord row)
        {
            record.ExpenseReportId = row.GetInt32(row.GetOrdinal("ExpenseReportId"));
            record.ItemNumber = row.GetInt32(row.GetOrdinal("ItemNumber"));
            record.Date = row.GetDateTime(row.GetOrdinal("Date"));
            record.ExpenseType = TranslateExpenseType(row.GetString(row.GetOrdinal("ExpenseType")));
            record.Value = row.IsDBNull(row.GetOrdinal("Value")) ? 0 : row.GetDecimal(row.GetOrdinal("Value"));
            record.Description = row.IsDBNull(row.GetOrdinal("Description")) ? null : row.GetString(row.GetOrdinal("Description"));
        }

        public static int ToItemNumber(IDataRecord reader)
        {
            return reader.GetInt32(reader.GetOrdinal("ItemNumber"));
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
            }
            throw new DataProviderException("Invalid record expense type.");
        }

        public static string TranslateExpenseType(ExpenseType expenseType)
        {
            switch (expenseType)
            {
                case ExpenseType.Office: return "O";
                case ExpenseType.Meal: return "M";
                case ExpenseType.HotelLodging: return "L";
                case ExpenseType.OtherLodging: return "L*";
                case ExpenseType.LandTransportation: return "TL";
                case ExpenseType.AirTransportation: return "TA";
                default: return "Ot"; //ExpenseType.Other
            }
        }
    }
}
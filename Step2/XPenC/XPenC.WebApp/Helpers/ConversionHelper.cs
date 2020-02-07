using System;
using System.Linq;
using Microsoft.Data.SqlClient;
using XPenC.WebApp.DataAccess.Schema;
using XPenC.WebApp.Models;

namespace XPenC.WebApp.Helpers
{
    public static class ConversionHelper
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

        public static ExpenseReport ToExpenseReport(SqlDataReader r)
        {
            return new ExpenseReport
            {
                Id = r.GetInt32(r.GetOrdinal("Id")),
                Client = r.IsDBNull(r.GetOrdinal("Client")) ? null : r.GetString(r.GetOrdinal("Client")),
                CreatedOn = r.GetDateTime(r.GetOrdinal("CreatedOn")),
                ModifiedOn = r.IsDBNull(r.GetOrdinal("ModifiedOn")) ? (DateTime?)null : r.GetDateTime(r.GetOrdinal("ModifiedOn")),
            };
        }

        public static void UpdateExpenseReport(ExpenseReport record, SqlDataReader r)
        {
            record.Id = r.GetInt32(r.GetOrdinal("Id"));
            record.Client = r.IsDBNull(r.GetOrdinal("Client")) ? null : r.GetString(r.GetOrdinal("Client"));
            record.CreatedOn = r.GetDateTime(r.GetOrdinal("CreatedOn"));
            record.ModifiedOn = r.GetDateTime(r.GetOrdinal("ModifiedOn"));
            record.MealTotal = r.GetDecimal(r.GetOrdinal("MealTotal"));
            record.Total = r.GetDecimal(r.GetOrdinal("Total"));
            if (!r.IsDBNull(r.GetOrdinal("ExpenseReportId")))
            {
                var item = new ExpenseReportItem
                {
                    ExpenseReportId = r.GetInt32(r.GetOrdinal("ExpenseReportId")),
                    ItemNumber = r.GetInt32(r.GetOrdinal("ItemNumber")),
                    Date = r.IsDBNull(r.GetOrdinal("Date")) ? (DateTime?)null : r.GetDateTime(r.GetOrdinal("Date")),
                    ExpenseType = TranslateExpenseType(r.IsDBNull(r.GetOrdinal("ExpenseType")) ? null : r.GetString(r.GetOrdinal("ExpenseType"))),
                    Value = r.IsDBNull(r.GetOrdinal("Value")) ? 0 : r.GetDecimal(r.GetOrdinal("Value")),
                    Description = r.IsDBNull(r.GetOrdinal("Description")) ? null : r.GetString(r.GetOrdinal("Description")),
                };
                record.Items.Add(item);
            }
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
                Items = source.Items.Select(ToExpenseReportItemDetails).ToList(),
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
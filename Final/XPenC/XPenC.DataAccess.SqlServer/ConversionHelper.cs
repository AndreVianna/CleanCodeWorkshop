using System;
using Microsoft.Data.SqlClient;
using XPenC.DataAccess.Contracts.Schema;

namespace XPenC.DataAccess.SqlServer
{
    internal static class ConversionHelper
    {
        public static ExpenseReportEntity ToExpenseReportEntity(SqlDataReader row)
        {
            return new ExpenseReportEntity
            {
                Id = row.GetInt32(row.GetOrdinal("Id")),
                Client = row.IsDBNull(row.GetOrdinal("Client")) ? null : row.GetString(row.GetOrdinal("Client")),
                CreatedOn = row.GetDateTime(row.GetOrdinal("CreatedOn")),
                ModifiedOn = row.IsDBNull(row.GetOrdinal("ModifiedOn")) ? (DateTime?)null : row.GetDateTime(row.GetOrdinal("ModifiedOn")),
            };
        }

        public static void ToExpenseReportWithItem(ExpenseReportEntity record, SqlDataReader row)
        {
            record.Id = row.GetInt32(row.GetOrdinal("Id"));
            record.Client = row.IsDBNull(row.GetOrdinal("Client")) ? null : row.GetString(row.GetOrdinal("Client"));
            record.CreatedOn = row.GetDateTime(row.GetOrdinal("CreatedOn"));
            record.ModifiedOn = row.IsDBNull(row.GetOrdinal("ModifiedOn")) ? (DateTime?)null : row.GetDateTime(row.GetOrdinal("ModifiedOn"));
            record.MealTotal = row.GetDecimal(row.GetOrdinal("MealTotal"));
            record.Total = row.GetDecimal(row.GetOrdinal("Total"));
            if (row.IsDBNull(row.GetOrdinal("ExpenseReportId")))
            {
                return;
            }
            record.Items.Add(ToExpenseReportItemEntity(row));
        }

        public static ExpenseReportItemEntity ToExpenseReportItemEntity(SqlDataReader row)
        {
            return new ExpenseReportItemEntity
            {
                ExpenseReportId = row.GetInt32(row.GetOrdinal("ExpenseReportId")),
                ItemNumber = row.GetInt32(row.GetOrdinal("ItemNumber")),
                Date = row.IsDBNull(row.GetOrdinal("Date")) ? (DateTime?)null : row.GetDateTime(row.GetOrdinal("Date")),
                ExpenseType = row.IsDBNull(row.GetOrdinal("ExpenseType")) ? null : row.GetString(row.GetOrdinal("ExpenseType")),
                Value = row.IsDBNull(row.GetOrdinal("Value")) ? 0 : row.GetDecimal(row.GetOrdinal("Value")),
                Description = row.IsDBNull(row.GetOrdinal("Description")) ? null : row.GetString(row.GetOrdinal("Description")),
            };
        }
    }
}
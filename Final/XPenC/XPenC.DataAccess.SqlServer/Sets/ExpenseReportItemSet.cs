using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using XPenC.DataAccess.Contracts;
using XPenC.DataAccess.Contracts.Schema;

namespace XPenC.DataAccess.SqlServer
{
    public class ExpenseReportItemTable : IExpenseReportItemTable
    {
        private readonly SqlConnectionHandler _sqlConnectionHandler;

        public ExpenseReportItemTable(SqlConnectionHandler sqlConnectionHandler)
        {
            _sqlConnectionHandler = sqlConnectionHandler;
        }

        public void Delete(int expenseReportId, int itemNumber)
        {
            var commandText = "DELETE FROM ExpenseReportItems " +
                              "WHERE ExpenseReportId = @id " +
                              "AND ItemNumber = @number";
            var parameters = new Dictionary<string, object>
            {
                ["id"] = expenseReportId,
                ["number"] = itemNumber,
            };

            _sqlConnectionHandler.Execute(commandText, parameters);
        }

        public IEnumerable<ExpenseReportItemEntity> GetAllFor(int expenseReportId)
        {
            var commandText = "SELECT * " +
                              "FROM ExpenseReportItems " +
                              "WHERE ExpenseReportId=@expenseReportId;";
            var parameters = new Dictionary<string, object> { ["expenseReportId"] = expenseReportId };
            
            return _sqlConnectionHandler.ReadMany(commandText, parameters, ConversionHelper.ToExpenseReportItemEntity);
        }

        public void Add(ExpenseReportItemEntity source)
        {
            var nextNumber = GetNextNumber(source.ExpenseReportId);
            var commandText = "INSERT INTO ExpenseReportItems " +
                              "(ExpenseReportId, ItemNumber, Date, ExpenseType, Value, Description) " +
                              "VALUES " +
                              "(@id, @number, @date, @expenseType, @value, @description)";
            var parameters = new Dictionary<string, object>
            {
                ["id"] = source.ExpenseReportId,
                ["number"] = nextNumber,
                ["date"] = source.Date,
                ["expenseType"] = source.ExpenseType,
                ["value"] = source.Value,
                ["description"] = source.Description,
            };

            _sqlConnectionHandler.Execute(commandText, parameters);
        }

        private int GetNextNumber(int id)
        {
            var commandText = "SELECT TOP 1 ItemNumber " +
                              "FROM ExpenseReportItems " +
                              "WHERE ExpenseReportId = @id " +
                              "ORDER BY 1 DESC";
            var parameters = new Dictionary<string, object>
            {
                ["id"] = id,
            };
            Func<SqlDataReader, int> readItemNumber = r => r.GetInt32(r.GetOrdinal("ItemNumber"));

            return _sqlConnectionHandler.ReadOne(commandText, parameters, readItemNumber) + 1;
        }
    }
}
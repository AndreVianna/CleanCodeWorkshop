using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using XPenC.DataAccess.Contracts.Schema;
using XPenC.DataAccess.Contracts.Sets;

namespace XPenC.DataAccess.SqlServer.Sets
{
    public class ExpenseReportItemSet : IExpenseReportItemSet
    {
        private readonly SqlConnectionHandler _sqlConnectionHandler;

        public ExpenseReportItemSet(SqlConnectionHandler sqlConnectionHandler)
        {
            _sqlConnectionHandler = sqlConnectionHandler;
        }

        public void DeleteFrom(int expenseReportId, int itemNumber)
        {
            const string commandText = "DELETE FROM ExpenseReportItems " +
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
            const string commandText = "SELECT * " +
                                       "FROM ExpenseReportItems " +
                                       "WHERE ExpenseReportId=@expenseReportId;";
            var parameters = new Dictionary<string, object> { ["expenseReportId"] = expenseReportId };
            
            return _sqlConnectionHandler.ReadMany(commandText, parameters, ConversionHelper.ToExpenseReportItemEntity);
        }

        public void AddTo(int expenseReportId, ExpenseReportItemEntity source)
        {
            var nextNumber = GetNextNumber(expenseReportId);
            const string commandText = "INSERT INTO ExpenseReportItems " +
                                       "(ExpenseReportId, ItemNumber, Date, ExpenseType, Value, Description) " +
                                       "VALUES " +
                                       "(@id, @number, @date, @expenseType, @value, @description)";
            var parameters = new Dictionary<string, object>
            {
                ["id"] = expenseReportId,
                ["number"] = nextNumber,
                ["date"] = source.Date,
                ["expenseType"] = source.ExpenseType,
                ["value"] = source.Value,
                ["description"] = source.Description,
            };

            _sqlConnectionHandler.Execute(commandText, parameters);

            source.ExpenseReportId = expenseReportId;
            source.ItemNumber = nextNumber;
        }

        private int GetNextNumber(int id)
        {
            const string commandText = "SELECT TOP 1 ItemNumber " +
                                       "FROM ExpenseReportItems " +
                                       "WHERE ExpenseReportId = @id " +
                                       "ORDER BY 1 DESC";
            var parameters = new Dictionary<string, object>
            {
                ["id"] = id,
            };

            return _sqlConnectionHandler.ReadOne(commandText, parameters, ReadItemNumber) + 1;
        }

        private static int ReadItemNumber(SqlDataReader reader)
        {
            return reader.GetInt32(reader.GetOrdinal("ItemNumber"));
        }
    }
}
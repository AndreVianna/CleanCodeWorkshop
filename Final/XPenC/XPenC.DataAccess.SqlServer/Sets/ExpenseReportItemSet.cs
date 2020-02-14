using System.Collections.Generic;
using XPenC.DataAccess.Contracts.Schema;
using XPenC.DataAccess.Contracts.Sets;
using static XPenC.DataAccess.SqlServer.ConversionHelper;

namespace XPenC.DataAccess.SqlServer.Sets
{
    public class ExpenseReportItemSet : IExpenseReportItemSet
    {
        private readonly ISqlDataProvider _sqlDataProvider;

        public ExpenseReportItemSet(ISqlDataProvider sqlDataProvider)
        {
            _sqlDataProvider = sqlDataProvider;
        }

        public IEnumerable<ExpenseReportItemEntity> GetAllFor(int expenseReportId)
        {
            const string commandText = "SELECT * " +
                                       "FROM ExpenseReportItems " +
                                       "WHERE ExpenseReportId=@expenseReportId;";
            var parameters = new Dictionary<string, object> { ["expenseReportId"] = expenseReportId };
            
            return _sqlDataProvider.ReadMany(commandText, parameters, ToExpenseReportItemEntity);
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

            _sqlDataProvider.Execute(commandText, parameters);

            source.ExpenseReportId = expenseReportId;
            source.ItemNumber = nextNumber;
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

            _sqlDataProvider.Execute(commandText, parameters);
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

            return _sqlDataProvider.ReadOne(commandText, parameters, ToItemNumber) + 1;
        }
    }
}
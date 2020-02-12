using System;
using System.Collections.Generic;
using System.Linq;
using XPenC.DataAccess.Contracts;
using XPenC.DataAccess.Contracts.Schema;

namespace XPenC.DataAccess.SqlServer
{
    public class ExpenseReportSet : IExpenseReportSet
    {
        private readonly SqlConnectionHandler _sqlConnectionHandler;
        private readonly IExpenseReportItemTable _itemTable;

        public ExpenseReportSet(SqlConnectionHandler sqlConnectionHandler, IExpenseReportItemTable itemTable)
        {
            _sqlConnectionHandler = sqlConnectionHandler;
            _itemTable = itemTable;
        }

        public ExpenseReportEntity CreateRecordWithDefaults()
        {
            var now = DateTime.Now;
            return new ExpenseReportEntity
            {
                CreatedOn = now,
                ModifiedOn = now,
            };
        }

        public void Add(ExpenseReportEntity source)
        {
            var commandText = "INSERT INTO ExpenseReports " +
                              "(CreatedOn, ModifiedOn, MealTotal, Total) " +
                              "VALUES " +
                              "(@created, @modified, @mealTotal, @total);" +
                              "SELECT SCOPE_IDENTITY();";
            var parameters = new Dictionary<string, object>
            {
                ["created"] = source.CreatedOn,
                ["modified"] = (object)source.ModifiedOn ?? DBNull.Value,
                ["mealTotal"] = source.MealTotal,
                ["total"] = source.Total,
            };
            source.Id = _sqlConnectionHandler.ExecuteWithResult(commandText, parameters);

            foreach (var item in source.Items)
            {
                _itemTable.Add(item);
            }
        }

        public IEnumerable<ExpenseReportEntity> GetAll()
        {
            var commandText = "SELECT * " +
                              "FROM ExpenseReports " +
                              "ORDER BY ModifiedOn DESC;";
            
            return _sqlConnectionHandler.ReadMany(commandText, ConversionHelper.ToExpenseReportEntity).ToList();
        }

        public ExpenseReportEntity Find(int id)
        {
            var result = new ExpenseReportEntity();
            var commandText = "SELECT * " +
                              "FROM ExpenseReports r " +
                              "LEFT JOIN ExpenseReportItems i ON r.Id = i.ExpenseReportId " +
                              "WHERE r.Id=@id;";
            var parameters = new Dictionary<string, object> { ["id"] = id };
            
            return !_sqlConnectionHandler.TryUpdate(result, commandText, parameters, ConversionHelper.UpdateExpenseReport)
                ? null
                : result;
        }

        public void Update(ExpenseReportEntity source)
        {
            var commandText = "UPDATE ExpenseReports SET " +
                              "Client = @client, " +
                              "ModifiedOn = @modifiedOn, " +
                              "Total = @total, " +
                              "MealTotal = @mealTotal " +
                              "WHERE Id = @id";
            var parameters = new Dictionary<string, object>
            {
                ["id"] = source.Id,
                ["client"] = (object)source.Client ?? DBNull.Value,
                ["modifiedOn"] = (object)source.ModifiedOn ?? DBNull.Value,
                ["total"] = source.Total,
                ["mealTotal"] = source.MealTotal,
            };
            _sqlConnectionHandler.Execute(commandText, parameters);

            var existingItems = _itemTable.GetAllFor(source.Id).ToList();
            var existingItemNumbers = existingItems.Select(i => i.ItemNumber).ToList();

            var sourceItems = source.Items.ToList();
            var inputItemNumbers = source.Items.Select(i => i.ItemNumber).ToArray();

            var itemsToRemove = existingItemNumbers.Except(inputItemNumbers).ToList();
            var itemsToAdd = inputItemNumbers.Except(existingItemNumbers).ToList();

            itemsToRemove.ForEach(i => _itemTable.Delete(source.Id, i));
            itemsToAdd.ForEach(i => _itemTable.Add(sourceItems.Find(r => r.ItemNumber == i)));
        }

        public void Delete(int id)
        {
            var commandText = "DELETE FROM ExpenseReports WHERE Id=@id;";
            var parameters = new Dictionary<string, object>
            {
                ["id"] = id,
            };
            _sqlConnectionHandler.Execute(commandText, parameters);
        }
    }
}
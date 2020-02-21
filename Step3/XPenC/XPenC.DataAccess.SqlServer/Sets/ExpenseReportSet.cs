using System;
using System.Collections.Generic;
using System.Linq;
using XPenC.DataAccess.Contracts.Schema;
using XPenC.DataAccess.Contracts.Sets;
using static XPenC.DataAccess.SqlServer.ConversionHelper;

namespace XPenC.DataAccess.SqlServer.Sets
{
    public class ExpenseReportSet : IExpenseReportSet
    {
        private readonly ISqlDataProvider _sqlDataProvider;
        private readonly IExpenseReportItemSet _itemSet;

        public ExpenseReportSet(ISqlDataProvider sqlDataProvider, IExpenseReportItemSet itemSet)
        {
            _sqlDataProvider = sqlDataProvider;
            _itemSet = itemSet;
        }

        private static ExpenseReportEntity CreateRecordWithDefaults()
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
            const string commandText = "INSERT INTO ExpenseReports " +
                                       "(CreatedOn, ModifiedOn, Client, MealTotal, Total) " +
                                       "VALUES " +
                                       "(@created, @modified, @client, @mealTotal, @total);" +
                                       "SELECT SCOPE_IDENTITY();";
            var parameters = new Dictionary<string, object>
            {
                ["created"] = source.CreatedOn,
                ["modified"] = (object)source.ModifiedOn ?? DBNull.Value,
                ["client"] = (object)source.Client ?? DBNull.Value,
                ["mealTotal"] = source.MealTotal,
                ["total"] = source.Total,
            };
            source.Id = _sqlDataProvider.ExecuteWithResult(commandText, parameters);

            foreach (var item in source.Items)
            {
                _itemSet.AddTo(source.Id, item);
            }
        }

        public IEnumerable<ExpenseReportEntity> GetAll()
        {
            const string commandText = "SELECT * " +
                                       "FROM ExpenseReports " +
                                       "ORDER BY ModifiedOn DESC;";

            return _sqlDataProvider.ReadMany(commandText, ToExpenseReportEntity).ToList();
        }

        public ExpenseReportEntity Find(int id)
        {
            var result = CreateRecordWithDefaults();
            const string commandText = "SELECT * " +
                                       "FROM ExpenseReports r " +
                                       "LEFT JOIN ExpenseReportItems i ON r.Id = i.ExpenseReportId " +
                                       "WHERE r.Id=@id;";
            var parameters = new Dictionary<string, object> { ["id"] = id };
            
            return !_sqlDataProvider.TryReadManyInto(result, commandText, parameters, UpdateExpenseReport)
                ? null
                : result;
        }

        public void Update(ExpenseReportEntity source)
        {
            const string commandText = "UPDATE ExpenseReports SET " +
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
            _sqlDataProvider.Execute(commandText, parameters);

            var existingItems = _itemSet.GetAllFor(source.Id).ToList();
            var existingItemNumbers = existingItems.Select(i => i.ItemNumber).ToList();

            var sourceItems = source.Items.ToList();
            var inputItemNumbers = sourceItems.Select(i => i.ItemNumber);
            var itemsNumbersToRemove = existingItemNumbers.Except(inputItemNumbers).ToList();
            itemsNumbersToRemove.ForEach(i => _itemSet.DeleteFrom(source.Id, i));

            var itemsToAdd = sourceItems.Where(r => r.ItemNumber == 0).ToList();
            itemsToAdd.ForEach(i => _itemSet.AddTo(source.Id, i));
        }

        public void Delete(int id)
        {
            const string commandText = "DELETE FROM ExpenseReports WHERE Id=@id;";
            var parameters = new Dictionary<string, object>
            {
                ["id"] = id,
            };
            _sqlDataProvider.Execute(commandText, parameters);
        }
    }
}
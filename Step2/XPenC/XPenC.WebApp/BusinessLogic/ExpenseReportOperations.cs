using System;
using System.Collections.Generic;
using System.Linq;
using XPenC.WebApp.DataAccess;
using XPenC.WebApp.DataAccess.Schema;
using XPenC.WebApp.Helpers;

namespace XPenC.WebApp.BusinessLogic
{
    public class ExpenseReportOperations
    {
        private readonly ConnectionHandler _connectionHandler;

        public ExpenseReportOperations(ConnectionHandler connectionHandler)
        {
            _connectionHandler = connectionHandler;
        }

        public ExpenseReport CreateExpenseReportWithDefaults()
        {
            var now = DateTime.Now;
            return new ExpenseReport
            {
                CreatedOn = now,
                ModifiedOn = now,
            };
        }

        public void AddExpenseReport(ExpenseReport newExpenseReport)
        {
            var commandText = "INSERT INTO ExpenseReports " +
                              "(CreatedOn, ModifiedOn, MealTotal, Total) " +
                              "VALUES " +
                              "(@created, @modified, @mealTotal, @total);" +
                              "SELECT SCOPE_IDENTITY();";
            var command = _connectionHandler.CreateNonQueryCommand(commandText, new Dictionary<string, object>
            {
                ["created"] = newExpenseReport.CreatedOn,
                ["modified"] = newExpenseReport.ModifiedOn,
                ["mealTotal"] = newExpenseReport.MealTotal,
                ["total"] = newExpenseReport.Total,
            });
            newExpenseReport.Id = command.ExecuteWithResult();
        }

        public List<ExpenseReport> GetExpenseReportList()
        {
            var commandText = "SELECT * " +
                              "FROM ExpenseReports " +
                              "ORDER BY ModifiedOn DESC;";
            var command = _connectionHandler.CreateQueryCommand<ExpenseReport>(commandText);
            return command.ReadMany(ConversionHelper.ToExpenseReport).ToList();
        }

        public ExpenseReport GetExistingReport(int id)
        {
            var commandText = "SELECT * " +
                              "FROM ExpenseReports r " +
                              "LEFT JOIN ExpenseReportItems i ON r.Id = i.ExpenseReportId " +
                              "WHERE r.Id=@id;";
            var command = _connectionHandler.CreateQueryCommand<ExpenseReport>(commandText, new Dictionary<string, object> { ["id"] = id });
            var result = new ExpenseReport();
            if (!command.TryUpdate(result, ConversionHelper.UpdateExpenseReport)) return null;
            return result;
        }

        public void ExecuteRemoveItem(ExpenseReport source, int itemNumber)
        {
            var commandText = "DELETE FROM ExpenseReportItems " +
                              "WHERE ExpenseReportId = @id " +
                              "AND ItemNumber = @number";
            var command = _connectionHandler.CreateNonQueryCommand(commandText, new Dictionary<string, object>
            {
                ["id"] = source.Id,
                ["number"] = itemNumber,
            });
            command.Execute();

            var itemToRemove = source.Items.ToList().Find(i => i.ItemNumber == itemNumber);
            source.Items.Remove(itemToRemove);
        }

        public void ExecuteAddItem(ExpenseReport source, ExpenseReportItem newItem)
        {
            var nextNumber = GetNextNumber(source.Id);
            var commandText = "INSERT INTO ExpenseReportItems " +
                              "(ExpenseReportId, ItemNumber, Date, ExpenseType, Value, Description) " +
                              "VALUES " +
                              "(@id, @number, @date, @expenseType, @value, @description)";
            var command = _connectionHandler.CreateNonQueryCommand(commandText, new Dictionary<string, object>
            {
                ["id"] = source.Id,
                ["number"] = nextNumber,
                ["date"] = newItem.Date,
                ["expenseType"] = newItem.ExpenseType,
                ["value"] = newItem.Value,
                ["description"] = newItem.Description,
            });
            command.Execute();

            source.Items.Add(new ExpenseReportItem
            {
                ItemNumber = nextNumber,
                Date = newItem.Date,
                ExpenseType = newItem.ExpenseType,
                Value = newItem.Value,
            });
        }

        private int GetNextNumber(int id)
        {
            var commandText = "SELECT TOP 1 ItemNumber " +
                              "FROM ExpenseReportItems " +
                              "WHERE ExpenseReportId = @id " +
                              "ORDER BY 1 DESC";
            var command = _connectionHandler.CreateQueryCommand<int>(commandText, new Dictionary<string, object>
            {
                ["id"] = id,
            });
            var nextNumber = command.ReadOne(r => r.GetInt32(r.GetOrdinal("ItemNumber")), 1);
            return nextNumber;
        }

        public void ExecuteExpenseReportUpdate(ExpenseReport source)
        {
            var commandText = "UPDATE ExpenseReports SET " +
                              "Client = @client " +
                              "WHERE Id = @id";
            var command = _connectionHandler.CreateNonQueryCommand(commandText, new Dictionary<string, object>
            {
                ["id"] = source.Id,
                ["client"] = source.Client,
            });
            command.Execute();
        }

        public void UpdateExpenseReportLastModificationDate(ExpenseReport source)
        {
            var commandText = "UPDATE ExpenseReports SET " +
                              "ModifiedOn = @modified " +
                              "WHERE Id = @id";
            var command = _connectionHandler.CreateNonQueryCommand(commandText, new Dictionary<string, object>
            {
                ["id"] = source.Id,
                ["modified"] = DateTime.Now,
            });
            command.Execute();
        }

        public void ExecuteDeleteReport(int id)
        {
            var commandText = "DELETE FROM ExpenseReports WHERE Id=@id;";
            var command = _connectionHandler.CreateNonQueryCommand(commandText, new Dictionary<string, object>
            {
                ["id"] = id,
            });
            command.Execute();
        }
    }
}
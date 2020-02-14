using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using XPenC.WebApp.Models;

namespace XPenC.WebApp.Controllers
{
    public class ExpenseReportsController : Controller
    {
        private readonly ConnectionHandler _connectionHandler;

        public ExpenseReportsController(IConfiguration configuration)
        {
            _connectionHandler = new ConnectionHandler(configuration);
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            _connectionHandler.Dispose();
        }

        // GET: ExpenseReports
        public IActionResult Index()
        {
            var result = GetReportList();

            return View(result);
        }

        // GET: ExpenseReports/Details/5
        public IActionResult Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var result = GetExistingReport(id.Value);

            if (result == null)
            {
                return NotFound();
            }

            return View(result);
        }

        // GET: ExpenseReports/Create
        public IActionResult Create()
        {
            try
            {
                var id = ExecuteCreateReport();

                _connectionHandler.CommitChanges();

                return RedirectToAction(nameof(Update), new { id });
            }
            catch (Exception ex)
            {
                return RedirectToAction("Error", "Home", new { message = $"{ex.GetType().Name}: {ex.Message}" });
            }
        }

        // GET: ExpenseReports/Update/5
        public IActionResult Update(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var result = ConvertToExpenseReportUpdateInput(GetExistingReport(id.Value));

            if (result == null)
            {
                return NotFound();
            }

            PrepareExpenseTypeDropdownList();
            return View(result);
        }

        // POST: ExpenseReports/Update/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Update(int? id, string action, ExpenseReportUpdateInput input)
        {
            try
            {
                if (id == null)
                {
                    return NotFound();
                }

                if (input == null || input.Id != id)
                {
                    return BadRequest("The update request data is not valid.");
                }

                var originalValue = GetExistingReport(id.Value);

                if (originalValue == null)
                {
                    return NotFound();
                }

                ValidateUpdateOperation(action, input);
                if (!ModelState.IsValid)
                {
                    RestoreInputItems(originalValue, input);
                    PrepareExpenseTypeDropdownList();
                    return View(input);
                }

                ExecuteUpdateReport(action, originalValue, input);

                _connectionHandler.CommitChanges();

                if (action == "Finish")
                {
                    return RedirectToAction(nameof(Index));
                }

                return RedirectToAction(nameof(Update), new {id});
            }
            catch (Exception ex)
            {
                return RedirectToAction("Error", "Home", new { message = $"{ex.GetType().Name}: {ex.Message}" });
            }
        }

        // GET: ExpenseReports/Delete/5
        public IActionResult Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var result = GetExistingReport(id.Value);

            if (result == null)
            {
                return NotFound();
            }

            return View(result);
        }

        // POST: ExpenseReports/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ExecuteDelete(int id)
        {
            try
            {
                ExecuteDeleteReport(id);

                _connectionHandler.CommitChanges();

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                return RedirectToAction("Error", "Home", new { message = $"{ex.GetType().Name}: {ex.Message}" });
            }
        }

        private List<ExpenseReportListItem> GetReportList()
        {
            var commandText = "SELECT * " +
                              "FROM ExpenseReports " +
                              "ORDER BY ModifiedOn DESC;";
            return _connectionHandler.ReadRowsAs(commandText, null, ConvertToExpenseReportListItem).ToList();
        }

        private static ExpenseReportListItem ConvertToExpenseReportListItem(SqlDataReader r)
        {
            return new ExpenseReportListItem
            {
                Id = r.GetInt32(r.GetOrdinal("Id")),
                Client = r.IsDBNull(r.GetOrdinal("Client")) ? null : r.GetString(r.GetOrdinal("Client")),
                CreatedOn = r.GetDateTime(r.GetOrdinal("CreatedOn")),
                ModifiedOn = r.GetDateTime(r.GetOrdinal("ModifiedOn")),
            };
        }

        private ExpenseReportDetails GetExistingReport(int id)
        {
            var commandText = "SELECT * " +
                              "FROM ExpenseReports r " +
                              "LEFT JOIN ExpenseReportItems i ON r.Id = i.ExpenseReportId " +
                              "WHERE r.Id=@id;";
            var parameters = new Dictionary<string, object> { ["id"] = id };
            return _connectionHandler.ReadRowsInto<ExpenseReportDetails>(commandText, parameters, UpdateExpenseReportDetails);
        }

        private static void UpdateExpenseReportDetails(ExpenseReportDetails record, SqlDataReader r)
        {
            record.Id = r.GetInt32(r.GetOrdinal("Id"));
            record.Client = r.IsDBNull(r.GetOrdinal("Client")) ? null : r.GetString(r.GetOrdinal("Client"));
            record.CreatedOn = r.GetDateTime(r.GetOrdinal("CreatedOn"));
            record.ModifiedOn = r.GetDateTime(r.GetOrdinal("ModifiedOn"));
            if (!r.IsDBNull(r.GetOrdinal("ExpenseReportId")))
            {
                var item = new ExpenseReportItemDetails
                {
                    Number = r.GetInt32(r.GetOrdinal("ItemNumber")),
                    Date = r.IsDBNull(r.GetOrdinal("Date")) ? (DateTime?)null : r.GetDateTime(r.GetOrdinal("Date")),
                    ExpenseType = TranslateExpenseType(r.IsDBNull(r.GetOrdinal("ExpenseType")) ? null : r.GetString(r.GetOrdinal("ExpenseType"))),
                    Value = r.IsDBNull(r.GetOrdinal("Value")) ? 0 : r.GetDecimal(r.GetOrdinal("Value")),
                    Description = r.IsDBNull(r.GetOrdinal("Description")) ? null : r.GetString(r.GetOrdinal("Description")),
                };
                record.Items.Add(item);
                record.MealTotal += item.ExpenseType == "Meal" ? (item.Value ?? 0m) : 0m;
                record.Total += item.Value ?? 0;
            }
        }

        private int ExecuteCreateReport()
        {
            var now = DateTime.Now;
            var commandText = "INSERT INTO ExpenseReports " +
                              "(CreatedOn, ModifiedOn, MealTotal, Total) " +
                              "VALUES " +
                              "(@created, @modified, @mealTotal, @total);" +
                              "SELECT SCOPE_IDENTITY();";
            var parameters = new Dictionary<string, object>
            {
                ["created"] = now,
                ["modified"] = now,
                ["mealTotal"] = 0,
                ["total"] = 0,
            };
            return _connectionHandler.ReadRowAs(commandText, parameters, r => Convert.ToInt32(r.GetDecimal(0)));
        }

        private static ExpenseReportUpdateInput ConvertToExpenseReportUpdateInput(ExpenseReportDetails source)
        {
            return new ExpenseReportUpdateInput
            {
                Id = source.Id,
                Client = source.Client,
                Items = new List<ExpenseReportItemDetails>(source.Items),
            };
        }

        private static string TranslateExpenseType(string expenseType)
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

        private void ValidateUpdateOperation(string action, ExpenseReportUpdateInput input)
        {
            if (string.IsNullOrWhiteSpace(input.Client))
            {
                ModelState.AddModelError("Client", "The client field is required.");
            }

            if (action == "Add")
            {
                if (input.NewItem.Date is null)
                {
                    ModelState.AddModelError("NewItem.Date", "The new expense date is required.");
                }

                if (input.NewItem.Date > DateTime.Now)
                {
                    ModelState.AddModelError("NewItem.Value", "The new expense date must not be in the future.");
                }

                if (string.IsNullOrWhiteSpace(input.NewItem.ExpenseType))
                {
                    ModelState.AddModelError("NewItem.ExpenseType", "The new expense type is required.");
                }

                if (input.NewItem.Value is null)
                {
                    ModelState.AddModelError("NewItem.Value", "The new expense value is required.");
                }

                if (input.NewItem.Value < 0)
                {
                    ModelState.AddModelError("NewItem.Value", "The new expense value must be greater than zero.");
                }
            }
        }

        private static void RestoreInputItems(ExpenseReportDetails originalValue, ExpenseReportUpdateInput input)
        {
            input.Items = new List<ExpenseReportItemDetails>(originalValue.Items);
        }

        private void PrepareExpenseTypeDropdownList()
        {
            ViewData["ExpenseTypes"] = new List<SelectListItem>
            {
                new SelectListItem("Office", "O"),
                new SelectListItem("Meal", "M"),
                new SelectListItem("Lodging (Hotel)", "L"),
                new SelectListItem("Lodging (Other)", "L*"),
                new SelectListItem("Transportation (Land)", "TL"),
                new SelectListItem("Transportation (Air)", "TA"),
                new SelectListItem("Other", "Ot"),
            };
        }

        private void ExecuteUpdateReport(string action, ExpenseReportDetails originalValue, ExpenseReportUpdateInput input)
        {
            var hasChanges = false;
            if (input.Client != originalValue.Client)
            {
                ExecuteUpdateMainRecordDetails(input);
                hasChanges = true;
            }

            if (action.StartsWith("Remove"))
            {
                var itemNumber = Convert.ToInt32(action.Replace("Remove", ""));
                ExecuteRemoveItem(originalValue, itemNumber);
                hasChanges = true;
            }

            if (action == "Add")
            {
                ExecuteAddItem(originalValue, input.NewItem);
                hasChanges = true;
            }

            if (hasChanges)
            {
                UpdateLastModificationDate(input);
            }
        }

        private void ExecuteRemoveItem(ExpenseReportDetails originalValue, int itemNumber)
        {
            var commandText = "DELETE FROM ExpenseReportItems " +
                          "WHERE ExpenseReportId = @id " +
                          "AND ItemNumber = @number";
            var parameters = new Dictionary<string, object>
            {
                ["id"] = originalValue.Id,
                ["number"] = itemNumber,
            };
            _connectionHandler.ExecuteNonQuery(commandText, parameters);

            var itemToRemove = originalValue.Items.ToList().Find(i => i.Number == itemNumber);
            originalValue.Items.Remove(itemToRemove);
        }

        private void ExecuteAddItem(ExpenseReportDetails originalValue, ExpenseReportItemDetails newItem)
        {
            var nextNumber = GetNextNumber(originalValue.Id);
            var commandText = "INSERT INTO ExpenseReportItems " +
                          "(ExpenseReportId, ItemNumber, Date, ExpenseType, Value, Description) " +
                          "VALUES " +
                          "(@id, @number, @date, @expenseType, @value, @description)";
            var parameters = new Dictionary<string, object>
            {
                ["id"] = originalValue.Id,
                ["number"] = nextNumber,
                ["date"] = newItem.Date,
                ["expenseType"] = newItem.ExpenseType,
                ["value"] = newItem.Value,
                ["description"] = newItem.Description,
            };
            _connectionHandler.ExecuteNonQuery(commandText, parameters);

            originalValue.Items.Add(new ExpenseReportItemDetails
            {
                Number = nextNumber,
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
            var parameters = new Dictionary<string, object>
            {
                ["id"] = id,
            };
            return _connectionHandler.ReadRowAs(commandText, parameters, r => r.GetInt32(r.GetOrdinal("ItemNumber")), 1);
        }

        private void ExecuteUpdateMainRecordDetails(ExpenseReportUpdateInput input)
        {
            var commandText = "UPDATE ExpenseReports SET " +
                              "Client = @client " +
                              "WHERE Id = @id";
            var parameters = new Dictionary<string, object>
            {
                ["id"] = input.Id,
                ["client"] = input.Client,
            };
            _connectionHandler.ExecuteNonQuery(commandText, parameters);
        }

        private void UpdateLastModificationDate(ExpenseReportUpdateInput input)
        {
            var commandText = "UPDATE ExpenseReports SET " +
                              "ModifiedOn = @modified " +
                              "WHERE Id = @id";
            var parameters = new Dictionary<string, object>
            {
                ["id"] = input.Id,
                ["modified"] = DateTime.Now,
            };
            _connectionHandler.ExecuteNonQuery(commandText, parameters);
        }

        private void ExecuteDeleteReport(int id)
        {
            var commandText = "DELETE FROM ExpenseReports WHERE Id=@id;";
            var parameters = new Dictionary<string, object>
            {
                ["id"] = id,
            };
            _connectionHandler.ExecuteNonQuery(commandText, parameters);
        }
    }

    public class ConnectionHandler : IDisposable
    {
        private readonly SqlTransaction _transaction;
        private readonly SqlConnection _connection;

        public ConnectionHandler(IConfiguration configuration)
        {
            var cs = configuration["ConnectionStrings:DataContext"];
            _connection = new SqlConnection(cs);
            _connection.Open();
            _transaction = _connection.BeginTransaction();
        }

        private bool _isDisposed;

        protected virtual void Dispose(bool isDisposing)
        {
            if (_isDisposed)
            {
                return;
            }

            if (isDisposing)
            {
                _transaction?.Dispose();
                _connection?.Dispose();
            }
            _isDisposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Security", "CA2100:Review SQL queries for security vulnerabilities", Justification = "Only used by pre-defined queries.")]
        private SqlCommand CreateCommand(string commandText, IDictionary<string, object> parameters)
        {
            var command = _transaction.Connection.CreateCommand();
            command.CommandText = commandText;
            command.Transaction = _transaction;
            if (parameters != null && parameters.Count > 0)
            {
                foreach (var parameter in parameters)
                {
                    command.Parameters.AddWithValue(parameter.Key, parameter.Value ?? DBNull.Value);
                }
            }
            return command;
        }

        public T ReadRowAs<T>(string commandText, IDictionary<string, object> parameters, Func<SqlDataReader, T> convertResult, T defaultValue = default)
        {
            using (var command = CreateCommand(commandText, parameters))
            {
                using (var row = command.ExecuteReader())
                {
                    if (row.Read())
                    {
                        return convertResult(row);
                    }

                    return defaultValue;
                }
            }
        }

        public IEnumerable<T> ReadRowsAs<T>(string commandText, IDictionary<string, object> parameters, Func<SqlDataReader, T> convertResult)
        {
            var result = new List<T>();
            using (var command = CreateCommand(commandText, parameters))
            {
                using (var row = command.ExecuteReader())
                {
                    while (row.Read())
                    {
                        result.Add(convertResult(row));
                    }
                }
            }
            return result;
        }

        public T ReadRowsInto<T>(string commandText, IDictionary<string, object> parameters, Action<T, SqlDataReader> updateResult)
            where T : class, new()
        {
            var result = new T();
            using (var command = CreateCommand(commandText, parameters))
            {
                using (var row = command.ExecuteReader())
                {
                    if (!row.HasRows)
                    {
                        return null;
                    }

                    while (row.Read())
                    {
                        updateResult(result, row);
                    }
                }
            }
            return result;
        }

        public void ExecuteNonQuery(string commandText, IDictionary<string, object> parameters)
        {
            using (var command = CreateCommand(commandText, parameters))
            {
                command.ExecuteNonQuery();
            }
        }

        public void CommitChanges()
        {
            _transaction.Commit();
        }
    }
}

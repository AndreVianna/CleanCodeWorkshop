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
                return NotFound();

            var result = GetExistingReportForDisplay(id.Value);

            if (result == null)
                return NotFound();

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
                return RedirectToAction("Error", "Home", new { message = ex.Message });
            }
        }

        // GET: ExpenseReports/Update/5
        public IActionResult Update(int? id)
        {
            if (id == null)
                return NotFound();

            var result = ConvertToExpenseReportUpdateInput(GetExistingReportForDisplay(id.Value));

            if (result == null)
                return NotFound();

            PrepareExpenseTypeDropdownList();
            return View(result);
        }

        // POST: ExpenseReports/Update/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Update(int? id, string action, ExpenseReportUpdateInput input)
        {
            if (id == null)
                return NotFound();

            if (input == null || input.Id != id)
                return BadRequest("The update request data is not valid.");

            try
            {
                var originalValue = GetExistingReportForDisplay(id.Value);

                if (originalValue == null)
                    return NotFound();

                ValidateUpdateOperation(action, input);
                if (!ModelState.IsValid)
                {
                    RestoreInputItems(originalValue, input);
                    PrepareExpenseTypeDropdownList();
                    return View(input);
                }

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

                _connectionHandler.CommitChanges();

                if (action == "Finish")
                    return RedirectToAction(nameof(Index));

                return RedirectToAction(nameof(Update), new {id});
            }
            catch (Exception ex)
            {
                return RedirectToAction("Error", "Home", new { message = ex.Message });
            }
        }

        // GET: ExpenseReports/Delete/5
        public IActionResult Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var result = GetExistingReportForDisplay(id.Value);

            if (result == null)
                return NotFound();

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
            }
            catch (Exception ex)
            {
                return RedirectToAction("Error", "Home", new { message = ex.Message });
            }
            return RedirectToAction(nameof(Index));
        }

        private List<ExpenseReportListItem> GetReportList()
        {
            var commandText = "SELECT * " +
                              "FROM ExpenseReports " +
                              "ORDER BY ModifiedOn DESC;";
            var command = _connectionHandler.CreateCommand(commandText);
            return command.ReadRowsAs(ConvertToExpenseReportListItem).ToList();
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

        private ExpenseReportDetails GetExistingReportForDisplay(int id)
        {
            var commandText = "SELECT * " +
                              "FROM ExpenseReports r " +
                              "LEFT JOIN ExpenseReportItems i ON r.Id = i.ExpenseReportId " +
                              "WHERE r.Id=@id;";
            var command = _connectionHandler.CreateCommand(commandText, new Dictionary<string, object> { ["id"] = id });
            return command.ReadRowsInto<ExpenseReportDetails>(UpdateExpenseReportDetails);
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
            var command = _connectionHandler.CreateCommand(commandText, new Dictionary<string, object>
            {
                ["created"] = now,
                ["modified"] = now,
                ["mealTotal"] = 0,
                ["total"] = 0,
            });
            return command.ReadRowAs(r => Convert.ToInt32(r.GetDecimal(0)));
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

        private void ExecuteRemoveItem(ExpenseReportDetails originalValue, int itemNumber)
        {
            var commandText = "DELETE FROM ExpenseReportItems " +
                          "WHERE ExpenseReportId = @id " +
                          "AND ItemNumber = @number";
            var command = _connectionHandler.CreateCommand(commandText, new Dictionary<string, object>
            {
                ["id"] = originalValue.Id,
                ["number"] = itemNumber,
            });
            command.ExecuteNonQuery();

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
            var command = _connectionHandler.CreateCommand(commandText, new Dictionary<string, object>
            {
                ["id"] = originalValue.Id,
                ["number"] = nextNumber,
                ["date"] = newItem.Date,
                ["expenseType"] = newItem.ExpenseType,
                ["value"] = newItem.Value,
                ["description"] = newItem.Description,
            });
            command.ExecuteNonQuery();

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
            var command = _connectionHandler.CreateCommand(commandText, new Dictionary<string, object>
            {
                ["id"] = id,
            });
            var nextNumber = command.ReadRowAs(r => r.GetInt32(r.GetOrdinal("ItemNumber")), 1);
            return nextNumber;
        }

        private void ExecuteUpdateMainRecordDetails(ExpenseReportUpdateInput input)
        {
            var commandText = "UPDATE ExpenseReports SET " +
                              "Client = @client " +
                              "WHERE Id = @id";
            var command = _connectionHandler.CreateCommand(commandText, new Dictionary<string, object>
            {
                ["id"] = input.Id,
                ["client"] = input.Client,
            });
            command.ExecuteNonQuery();
        }

        private void UpdateLastModificationDate(ExpenseReportUpdateInput input)
        {
            var commandText = "UPDATE ExpenseReports SET " +
                              "ModifiedOn = @modified " +
                              "WHERE Id = @id";
            var command = _connectionHandler.CreateCommand(commandText, new Dictionary<string, object>
            {
                ["id"] = input.Id,
                ["modified"] = DateTime.Now,
            });
            command.ExecuteNonQuery();
        }

        private void ExecuteDeleteReport(int id)
        {
            var commandText = "DELETE FROM ExpenseReports WHERE Id=@id;";
            var command = _connectionHandler.CreateCommand(commandText, new Dictionary<string, object>
            {
                ["id"] = id,
            });
            command.ExecuteNonQuery();
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
            if (_isDisposed) return;
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

        public SqlCommandWrapper CreateCommand(string commandText, IDictionary<string, object> parameters = null)
        {
            using (var command = _transaction.Connection.CreateCommand())
            {
                command.CommandText = commandText;
                command.Transaction = _transaction;
                if (parameters != null && parameters.Count > 0)
                {
                    foreach (var parameter in parameters)
                    {
                        command.Parameters.AddWithValue(parameter.Key, parameter.Value);
                    }
                }
                return new SqlCommandWrapper(command);
            }
        }

        public void CommitChanges()
        {
            _transaction.Commit();
        }
    }

    public class SqlCommandWrapper
    {
        private readonly SqlCommand _command;

        public SqlCommandWrapper(SqlCommand command)
        {
            _command = command;
        }

        
        public T ReadRowAs<T>(Func<SqlDataReader, T> convertResult, T defaultValue = default)
        {
            using (var r = _command.ExecuteReader())
            {
                if (r.Read())
                    return convertResult(r);
                return defaultValue;
            }
        }

        public IEnumerable<T> ReadRowsAs<T>(Func<SqlDataReader, T> convertResult)
        {
            var result = new List<T>();
            using (var r = _command.ExecuteReader())
            {
                while (r.Read())
                {
                    result.Add(convertResult(r));
                }
            }
            return result;
        }

        public T ReadRowsInto<T>(Action<T, SqlDataReader> updateResult)
            where T : class, new()
        {
            var result = new T();
            using (var r = _command.ExecuteReader())
            {
                if (!r.HasRows)
                    return null;

                while (r.Read())
                {
                    updateResult(result, r);
                }
            }
            return result;
        }

        public void ExecuteNonQuery()
        {
            _command.ExecuteNonQuery();
        }
    }
}

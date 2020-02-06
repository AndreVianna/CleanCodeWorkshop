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
        private readonly IConfiguration _configuration;

        public ExpenseReportsController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // GET: ExpenseReports
        public IActionResult Index()
        {
            var cs = _configuration["ConnectionStrings:DataContext"];
            var db = new SqlConnection(cs);
            db.Open();
            try
            {
                var rslt = new List<ExpenseReportListItem>();
                using (var cmd = db.CreateCommand())
                {
                    cmd.CommandText = "SELECT * FROM ExpenseReports ORDER BY ModifiedOn DESC;";
                    using (var r = cmd.ExecuteReader())
                    {
                        while (r.Read())
                        {
                            var item = new ExpenseReportListItem
                            {
                                Id = r.GetInt32(r.GetOrdinal("Id")),
                                Client = r.IsDBNull(r.GetOrdinal("Client")) ? null : r.GetString(r.GetOrdinal("Client")),
                                CreatedOn = r.GetDateTime(r.GetOrdinal("CreatedOn")),
                                ModifiedOn = r.GetDateTime(r.GetOrdinal("ModifiedOn")),
                            };
                            rslt.Add(item);
                        }
                    }
                }
                return View(rslt);
            }
            finally
            {
                db.Close();
            }
        }

        // GET: ExpenseReports/Details/5
        public IActionResult Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cs = _configuration["ConnectionStrings:DataContext"];
            var db = new SqlConnection(cs);
            db.Open();
            try
            {
                var rslt = new ExpenseReportDetails();
                using (var cmd = db.CreateCommand())
                {
                    cmd.CommandText = "SELECT *" +
                                      "FROM ExpenseReports r " +
                                      "LEFT JOIN ExpenseReportItems i ON r.Id = i.ExpenseReportId " +
                                      "WHERE r.Id=@id;";
                    cmd.Parameters.AddWithValue("id", id);
                    using (var r = cmd.ExecuteReader())
                    {
                        if (!r.HasRows)
                        {
                            return NotFound();
                        }
                        var first = true;
                        var items = new List<ExpenseReportDetails.ExpenseReportItem>();
                        while (r.Read())
                        {
                            if (first)
                            {
                                rslt.Id = id.Value;
                                rslt.Client = r.IsDBNull(r.GetOrdinal("Client")) ? null : r.GetString(r.GetOrdinal("Client"));
                                rslt.CreatedOn = r.GetDateTime(r.GetOrdinal("CreatedOn"));
                                rslt.ModifiedOn = r.GetDateTime(r.GetOrdinal("ModifiedOn"));
                                first = false;
                            }

                            if (!r.IsDBNull(r.GetOrdinal("ExpenseReportId")))
                            {
                                var item = new ExpenseReportDetails.ExpenseReportItem
                                {
                                    Number = r.GetInt32(r.GetOrdinal("ItemNumber")),
                                    Date = r.IsDBNull(r.GetOrdinal("Date")) ? (DateTime?)null : r.GetDateTime(r.GetOrdinal("Date")),
                                    ExpenseType = r.IsDBNull(r.GetOrdinal("ExpenseType")) ? null : r.GetString(r.GetOrdinal("ExpenseType")),
                                    Value = r.IsDBNull(r.GetOrdinal("Value")) ? 0 : r.GetDecimal(r.GetOrdinal("Value")),
                                };
                                switch (item.ExpenseType)
                                {
                                    case "O": item.ExpenseType = "Office"; break;
                                    case "M": item.ExpenseType = "Meal"; break;
                                    case "L": item.ExpenseType = "Lodging (Hotel)"; break;
                                    case "L*": item.ExpenseType = "Lodging (Other)"; break;
                                    case "TL": item.ExpenseType = "Transportation (Land)"; break;
                                    case "TA": item.ExpenseType = "Transportation (Air)"; break;
                                    case "Ot": item.ExpenseType = "Other"; break;
                                    default: item.ExpenseType = "Unknown"; break;
                                }
                                items.Add(item);
                            }
                        }
                        rslt.Items = items.ToArray();
                    }
                }

                rslt.MealTotal = rslt.Items.Where(i => i.ExpenseType == "Meal").Sum(i => i.Value ?? 0);
                rslt.Total = rslt.Items.Sum(i => i.Value ?? 0);
                return View(rslt);
            }
            finally
            {
                db.Close();
            }
        }


        // GET: ExpenseReports/Create
        public IActionResult Create()
        {
            var id = 0;
            var cs = _configuration["ConnectionStrings:DataContext"];
            var db = new SqlConnection(cs);
            db.Open();
            try
            {
                var trans = db.BeginTransaction();
                try
                {
                    using (var cmd = db.CreateCommand())
                    {
                        cmd.CommandText = "INSERT INTO ExpenseReports " +
                                          "(CreatedOn, ModifiedOn, MealTotal, Total) " +
                                          "VALUES " +
                                          "(@created, @modified, @mealTotal, @total);" +
                                          "SELECT SCOPE_IDENTITY();";
                        var now = DateTime.Now;
                        cmd.Parameters.AddWithValue("created", now);
                        cmd.Parameters.AddWithValue("modified", now);
                        cmd.Parameters.AddWithValue("mealTotal", 0);
                        cmd.Parameters.AddWithValue("total", 0);
                        cmd.Transaction = trans;
                        using (var r = cmd.ExecuteReader())
                        {
                            while (r.Read())
                            {
                                id = Convert.ToInt32(r.GetDecimal(0));
                            }
                        }
                    }
                    trans.Commit();
                }
                catch (Exception ex)
                {
                    trans.Rollback();
                    return RedirectToAction("Error", "Home", new { message = ex.Message });
                }
            }
            finally
            {
                db.Close();
            }
            return RedirectToAction(nameof(Update), new { id });
        }

        // GET: ExpenseReports/Update/5
        public IActionResult Update(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cs = _configuration["ConnectionStrings:DataContext"];
            var db = new SqlConnection(cs);
            db.Open();
            try
            {
                var rslt = new ExpenseReportUpdateInput();
                using (var cmd = db.CreateCommand())
                {
                    cmd.CommandText = "SELECT *" +
                                      "FROM ExpenseReports r " +
                                      "LEFT JOIN ExpenseReportItems i ON r.Id = i.ExpenseReportId " +
                                      "WHERE r.Id=@id;";
                    cmd.Parameters.AddWithValue("id", id);
                    using (var r = cmd.ExecuteReader())
                    {
                        if (!r.HasRows)
                        {
                            return NotFound();
                        }
                        var first = true;
                        var items = new List<ExpenseReportUpdateInput.ExpenseReportItem>();
                        while (r.Read())
                        {
                            if (first)
                            {
                                rslt.Id = id.Value;
                                rslt.Client = r.IsDBNull(r.GetOrdinal("Client")) ? null : r.GetString(r.GetOrdinal("Client"));
                                first = false;
                            }

                            if (!r.IsDBNull(r.GetOrdinal("ExpenseReportId")))
                            {
                                var item = new ExpenseReportUpdateInput.ExpenseReportItem
                                {
                                    Number = r.GetInt32(r.GetOrdinal("ItemNumber")),
                                    Date = r.IsDBNull(r.GetOrdinal("Date")) ? (DateTime?)null : r.GetDateTime(r.GetOrdinal("Date")),
                                    ExpenseType = r.IsDBNull(r.GetOrdinal("ExpenseType")) ? null : r.GetString(r.GetOrdinal("ExpenseType")),
                                    Value = r.IsDBNull(r.GetOrdinal("Value")) ? 0 : r.GetDecimal(r.GetOrdinal("Value")),
                                };
                                switch (item.ExpenseType)
                                {
                                    case "O": item.ExpenseType = "Office"; break;
                                    case "M": item.ExpenseType = "Meal"; break;
                                    case "L": item.ExpenseType = "Lodging (Hotel)"; break;
                                    case "L*": item.ExpenseType = "Lodging (Other)"; break;
                                    case "TL": item.ExpenseType = "Transportation (Land)"; break;
                                    case "TA": item.ExpenseType = "Transportation (Air)"; break;
                                    case "Ot": item.ExpenseType = "Other"; break;
                                    default: item.ExpenseType = "Unknown"; break;
                                }
                                items.Add(item);
                            }
                        }
                        rslt.Items = items.ToArray();
                    }
                }
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
                return View(rslt);
            }
            finally
            {
                db.Close();
            }
        }

        // POST: ExpenseReports/Update/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Update(int? id, string action, ExpenseReportUpdateInput input)
        {
            if (id == null)
            {
                return NotFound();
            }

            if (input == null || input.Id != id)
            {
                return RedirectToAction("Error", "Home", new { message = "Bad request while updating expense report." });
            }

            var cs = _configuration["ConnectionStrings:DataContext"];
            var db = new SqlConnection(cs);
            db.Open();
            try
            {
                var oldValues = new ExpenseReportUpdateInput();
                using (var cmd = db.CreateCommand())
                {
                    cmd.CommandText = "SELECT *" +
                                      "FROM ExpenseReports r " +
                                      "LEFT JOIN ExpenseReportItems i ON r.Id = i.ExpenseReportId " +
                                      "WHERE r.Id=@id;";
                    cmd.Parameters.AddWithValue("id", id);
                    using (var r = cmd.ExecuteReader())
                    {
                        if (!r.HasRows)
                        {
                            return NotFound();
                        }
                        var first = true;
                        var items = new List<ExpenseReportUpdateInput.ExpenseReportItem>();
                        while (r.Read())
                        {
                            if (first)
                            {
                                oldValues.Id = id.Value;
                                oldValues.Client = r.IsDBNull(r.GetOrdinal("Client")) ? null : r.GetString(r.GetOrdinal("Client"));
                                first = false;
                            }

                            if (!r.IsDBNull(r.GetOrdinal("ExpenseReportId")))
                            {
                                items.Add(new ExpenseReportUpdateInput.ExpenseReportItem
                                {
                                    Number = r.GetInt32(r.GetOrdinal("ItemNumber")),
                                    Date = r.IsDBNull(r.GetOrdinal("Date")) ? (DateTime?)null : r.GetDateTime(r.GetOrdinal("Date")),
                                    ExpenseType = r.IsDBNull(r.GetOrdinal("ExpenseType")) ? null : r.GetString(r.GetOrdinal("ExpenseType")),
                                    Value = r.IsDBNull(r.GetOrdinal("Value")) ? 0 : r.GetDecimal(r.GetOrdinal("Value")),
                                });
                            }
                        }
                        oldValues.Items = items.ToArray();
                    }
                }

                var trans = db.BeginTransaction();
                try
                {
                    if (string.IsNullOrWhiteSpace(input.Client))
                    {
                        ModelState.AddModelError("Client", "The client field is required.");
                        return View(input);
                    }

                    var changed = false;

                    if (action.StartsWith("Remove"))
                    {
                        var number = Convert.ToInt32(action.Replace("Remove", ""));
                        using (var cmd = db.CreateCommand())
                        {
                            cmd.CommandText = "DELETE FROM ExpenseReportItems " +
                                              "WHERE ExpenseReportId = @id " +
                                              "AND ItemNumber = @number ";
                            cmd.Parameters.AddWithValue("id", id);
                            cmd.Parameters.AddWithValue("number", number);
                            cmd.Transaction = trans;
                            cmd.ExecuteNonQuery();
                        }

                        var items = oldValues.Items.ToList();
                        var removeItem = items.Find(i => i.Number == number);
                        items.Remove(removeItem);
                        input.Items = items.ToArray();
                        changed = true;
                    }

                    if (action == "Add")
                    {
                        var block = false;
                        if (input.NewItem.Date is null)
                        {
                            ModelState.AddModelError("NewItem.Date", "The new expense date is required.");
                            block = true;
                        }
                        if (input.NewItem.Date > DateTime.Now)
                        {
                            ModelState.AddModelError("NewItem.Value", "The new expense date must not be in the future.");
                            block = true;
                        }
                        if (string.IsNullOrWhiteSpace(input.NewItem.ExpenseType))
                        {
                            ModelState.AddModelError("NewItem.ExpenseType", "The new expense type is required.");
                            block = true;
                        }
                        if (input.NewItem.Value is null)
                        {
                            ModelState.AddModelError("NewItem.Value", "The new expense value is required.");
                            block = true;
                        }
                        if (input.NewItem.Value < 0)
                        {
                            ModelState.AddModelError("NewItem.Value", "The new expense value must be greater than zero.");
                            block = true;
                        }

                        if (block)
                        {
                            trans.Rollback();
                            input.Items = oldValues.Items;
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
                            return View(input);
                        }

                        var nextNumber = 1;
                        using (var cmd = db.CreateCommand())
                        {
                            cmd.CommandText = "SELECT TOP 1 ItemNumber " +
                                              "FROM ExpenseReportItems " +
                                              "WHERE ExpenseReportId = @id " +
                                              "ORDER BY 1 DESC ";
                            cmd.Parameters.AddWithValue("id", id);
                            cmd.Transaction = trans;
                            using (var r = cmd.ExecuteReader())
                            {
                                if (r.Read())
                                {
                                    nextNumber = r.GetInt32(0) + 1;
                                }
                            }
                        }

                        using (var cmd = db.CreateCommand())
                        {
                            cmd.CommandText = "INSERT INTO ExpenseReportItems " +
                                              "(ExpenseReportId, ItemNumber, Date, ExpenseType, Value) " +
                                              "VALUES " +
                                              "(@id, @number, @date, @expenseType, @value) ";
                            cmd.Parameters.AddWithValue("id", id);
                            cmd.Parameters.AddWithValue("number", nextNumber);
                            cmd.Parameters.AddWithValue("date", input.NewItem.Date);
                            cmd.Parameters.AddWithValue("expenseType", input.NewItem.ExpenseType);
                            cmd.Parameters.AddWithValue("value", input.NewItem.Value);
                            cmd.Transaction = trans;
                            cmd.ExecuteNonQuery();
                        }

                        var items = oldValues.Items.ToList();
                        items.Add(new ExpenseReportUpdateInput.ExpenseReportItem
                        {
                            Number = input.Items.Length + 1,
                            Date = input.NewItem.Date,
                            ExpenseType = input.NewItem.ExpenseType,
                            Value = input.NewItem.Value,
                        });
                        input.Items = items.ToArray();
                        input.NewItem = new ExpenseReportUpdateInput.ExpenseReportItem();

                        changed = true;
                    }

                    if (changed || input.Client != oldValues.Client)
                    {
                        using (var cmd = db.CreateCommand())
                        {
                            cmd.CommandText = "UPDATE ExpenseReports SET " +
                                              "Client = @client, " +
                                              "ModifiedOn = @modified " +
                                              "WHERE Id = @id";
                            cmd.Parameters.AddWithValue("id", id);
                            cmd.Parameters.AddWithValue("client", input.Client);
                            cmd.Parameters.AddWithValue("modified", DateTime.Now);
                            cmd.Transaction = trans;
                            cmd.ExecuteNonQuery();
                        }
                    }
                    trans.Commit();

                    if (action == "Finish")
                    {
                        return RedirectToAction(nameof(Index));
                    }
                    return RedirectToAction(nameof(Update), new { id });
                }
                catch (Exception ex)
                {
                    trans.Rollback();
                    throw;
                }
            }
            catch (Exception ex)
            {
                return RedirectToAction("Error", "Home", new { message = ex.Message });
            }
            finally
            {
                db.Close();
            }
        }

        // GET: ExpenseReports/Delete/5
        public IActionResult Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cs = _configuration["ConnectionStrings:DataContext"];
            var db = new SqlConnection(cs);
            db.Open();
            try
            {
                var rslt = new ExpenseReportDetails();
                using (var cmd = db.CreateCommand())
                {
                    cmd.CommandText = "SELECT * FROM ExpenseReports WHERE Id=@id;";
                    cmd.Parameters.AddWithValue("id", id);
                    using (var r = cmd.ExecuteReader())
                    {
                        if (r.Read())
                        {
                            rslt.Id = id.Value;
                            rslt.Client = r.IsDBNull(r.GetOrdinal("Client")) ? null : r.GetString(r.GetOrdinal("Client"));
                            rslt.CreatedOn = r.GetDateTime(r.GetOrdinal("CreatedOn"));
                            rslt.ModifiedOn = r.GetDateTime(r.GetOrdinal("ModifiedOn"));
                        }
                    }
                }
                return View(rslt);
            }
            finally
            {
                db.Close();
            }
        }

        // POST: ExpenseReports/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ExecuteDelete(int id)
        {
            var cs = _configuration["ConnectionStrings:DataContext"];
            var db = new SqlConnection(cs);
            db.Open();
            try
            {
                var trans = db.BeginTransaction();
                try
                {
                    using (var cmd = db.CreateCommand())
                    {
                        cmd.CommandText = "DELETE FROM ExpenseReports WHERE Id=@id;";
                        cmd.Parameters.AddWithValue("id", id);
                        cmd.Transaction = trans;
                        cmd.ExecuteNonQuery();
                    }
                    trans.Commit();
                }
                catch (Exception ex)
                {
                    trans.Rollback();
                    return RedirectToAction("Error", "Home", new { message = ex.Message });
                }
            }
            finally
            {
                db.Close();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}

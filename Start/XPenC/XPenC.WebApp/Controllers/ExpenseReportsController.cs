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
                var rslt = new List<ERListItem>();
                using (var cmd = db.CreateCommand())
                {
                    cmd.CommandText = "SELECT * FROM ExpenseReports ORDER BY ModifiedOn DESC;";
                    using (var r = cmd.ExecuteReader())
                    {
                        while (r.Read())
                        {
                            var item = new ERListItem
                            {
                                Id = r.GetInt32(0),
                                Name = r.IsDBNull(1) ? null : r.GetString(1),
                                Created = r.GetDateTime(2),
                                Changed = r.GetDateTime(2),
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
                var rslt = new ERDetails();
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
                        var items = new List<ERDetails.Item>();
                        while (r.Read())
                        {
                            if (first)
                            {
                                rslt.Id = id.Value;
                                rslt.Client = r.IsDBNull(1) ? null : r.GetString(1);
                                rslt.CreateDate = r.GetDateTime(2);
                                rslt.ChangeDate = r.GetDateTime(3);
                                first = false;
                            }

                            if (!r.IsDBNull(r.GetOrdinal("ExpenseReportId")))
                            {
                                var item = new ERDetails.Item
                                {
                                    ItemId = r.GetInt32(7),
                                    Date = r.IsDBNull(8) ? (DateTime?)null : r.GetDateTime(8),
                                    ExpenseType = r.IsDBNull(9) ? null : r.GetString(9),
                                    Value = r.IsDBNull(11) ? 0 : r.GetDecimal(11),
                                    Description = r.IsDBNull(10) ? null : r.GetString(10),
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

                rslt.Meal = rslt.Items.Where(i => i.ExpenseType == "Meal").Sum(i => i.Value ?? 0);
                rslt.SumIts = rslt.Items.Sum(i => i.Value ?? 0);
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
                var rslt = new ERUpdate();
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
                        var items = new List<ERUpdate.Item>();
                        while (r.Read())
                        {

                            if (first)
                            {
                                rslt.Id = id.Value;
                                rslt.ClientText = r.IsDBNull(1) ? null : r.GetString(1);
                                first = false;
                            }

                            if (!r.IsDBNull(6))
                            {
                                var item = new ERUpdate.Item
                                {
                                    Number = r.GetInt32(7),
                                    ItemDate = r.IsDBNull(8) ? (DateTime?)null : r.GetDateTime(8),
                                    Type = r.IsDBNull(9) ? null : r.GetString(9),
                                    Price = r.IsDBNull(11) ? 0 : r.GetDecimal(11),
                                    Description = r.IsDBNull(10) ? null : r.GetString(10),
                                };
                                switch (item.Type)
                                {
                                    case "O": item.Type = "Office"; break;
                                    case "M": item.Type = "Meal"; break;
                                    case "L": item.Type = "Lodging (Hotel)"; break;
                                    case "L*": item.Type = "Lodging (Other)"; break;
                                    case "TL": item.Type = "Transportation (Land)"; break;
                                    case "TA": item.Type = "Transportation (Air)"; break;
                                    case "Ot": item.Type = "Other"; break;
                                    default: item.Type = "Unknown"; break;
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
        public IActionResult Update(int? id, string action, ERUpdate input)
        {
            if (id == null)
            {
                return NotFound();
            }

            if (input == null || input.Id != id)
            {
                return BadRequest("The update request data is not valid.");
            }

            var cs = _configuration["ConnectionStrings:DataContext"];
            var db = new SqlConnection(cs);
            db.Open();
            try
            {
                var old = new ERUpdate();
                using (var cmd = db.CreateCommand())
                {
                    cmd.CommandText = "SELECT *" +
                                      "FROM ExpenseReports r " +
                                      "LEFT JOIN ExpenseReportItems i ON r.Id = i.ExpenseReportId " +
                                      "WHERE r.Id=@id;";
                    cmd.Parameters.AddWithValue("id", id);
                    using (var r = cmd.ExecuteReader())
                    {
                        var items = new List<ERUpdate.Item>();
                        var stop = true;
                        while (r.Read())
                        {
                            stop = false;
                            old.Id = id.Value;
                            old.ClientText = r.IsDBNull(1) ? null : r.GetString(1);
                            if (!r.IsDBNull(6))
                            {
                                var item =
                                new ERUpdate.Item
                                {
                                    Number = r.GetInt32(7),
                                    ItemDate = r.IsDBNull(8) ? (DateTime?)null : r.GetDateTime(8),
                                    Type = r.IsDBNull(9) ? null : r.GetString(9),
                                    Price = r.IsDBNull(11) ? 0 : r.GetDecimal(11),
                                    Description = r.IsDBNull(10) ? null : r.GetString(10),
                                };
                                switch (item.Type)
                                {
                                    case "O": item.Type = "Office"; break;
                                    case "M": item.Type = "Meal"; break;
                                    case "L": item.Type = "Lodging (Hotel)"; break;
                                    case "L*": item.Type = "Lodging (Other)"; break;
                                    case "TL": item.Type = "Transportation (Land)"; break;
                                    case "TA": item.Type = "Transportation (Air)"; break;
                                    case "Ot": item.Type = "Other"; break;
                                    default: item.Type = "Unknown"; break;
                                }

                                items.Add(item);
                            }
                        }
                        if (stop)
                        {
                            return NotFound();
                        }
                        old.Items = items.ToArray();
                    }
                }

                var trans = db.BeginTransaction();
                try
                {
                    if (string.IsNullOrWhiteSpace(input.ClientText))
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

                        var items = old.Items.ToList();
                        var removeItem = items.Find(i => i.Number == number);
                        items.Remove(removeItem);
                        input.Items = items.ToArray();
                        changed = true;
                    }

                    if (action == "Add")
                    {
                        var block = false;
                        if (input.NewItem.ItemDate is null)
                        {
                            ModelState.AddModelError("NewItem.Date", "The new expense date is required.");
                            block = true;
                        }
                        if (input.NewItem.ItemDate > DateTime.Now)
                        {
                            ModelState.AddModelError("NewItem.Value", "The new expense date must not be in the future.");
                            block = true;
                        }
                        if (string.IsNullOrWhiteSpace(input.NewItem.Type))
                        {
                            ModelState.AddModelError("NewItem.ExpenseType", "The new expense type is required.");
                            block = true;
                        }
                        if (input.NewItem.Price is null)
                        {
                            ModelState.AddModelError("NewItem.Value", "The new expense value is required.");
                            block = true;
                        }
                        if (input.NewItem.Price <= 0)
                        {
                            ModelState.AddModelError("NewItem.Value", "The new expense value must be greater than zero.");
                            block = true;
                        }

                        if (block)
                        {
                            trans.Rollback();
                            input.Items = old.Items;
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
                                              "(ExpenseReportId, ItemNumber, Date, ExpenseType, Value, Description) " +
                                              "VALUES " +
                                              "(@id, @number, @date, @expenseType, @value, @description) ";
                            cmd.Parameters.AddWithValue("id", id);
                            cmd.Parameters.AddWithValue("number", nextNumber);
                            cmd.Parameters.AddWithValue("date", input.NewItem.ItemDate);
                            cmd.Parameters.AddWithValue("expenseType", input.NewItem.Type);
                            cmd.Parameters.AddWithValue("value", input.NewItem.Price);
                            cmd.Parameters.AddWithValue("description", input.NewItem.Description);
                            cmd.Transaction = trans;
                            cmd.ExecuteNonQuery();
                        }

                        var items = old.Items.ToList();
                        items.Add(new ERUpdate.Item
                        {
                            Number = input.Items.Length + 1,
                            ItemDate = input.NewItem.ItemDate,
                            Type = input.NewItem.Type,
                            Price = input.NewItem.Price,
                            Description = input.NewItem.Description,
                        });
                        input.Items = items.ToArray();
                        input.NewItem = new ERUpdate.Item();

                        changed = true;
                    }

                    if (changed || input.ClientText != old.ClientText)
                    {
                        using (var cmd = db.CreateCommand())
                        {
                            cmd.CommandText = "UPDATE ExpenseReports SET " +
                                              "Client = @client, " +
                                              "ModifiedOn = @modified " +
                                              "WHERE Id = @id";
                            cmd.Parameters.AddWithValue("id", id);
                            cmd.Parameters.AddWithValue("client", input.ClientText);
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
                catch
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
                var rslt = new ERDetails();
                using (var cmd = db.CreateCommand())
                {
                    cmd.CommandText = "SELECT * FROM ExpenseReports WHERE Id=@id;";
                    cmd.Parameters.AddWithValue("id", id);
                    using (var r = cmd.ExecuteReader())
                    {
                        if (r.Read())
                        {
                            rslt.Id = id.Value;
                            rslt.Client = r.IsDBNull(1) ? null : r.GetString(1);
                            rslt.CreateDate = r.GetDateTime(2);
                            rslt.ChangeDate = r.GetDateTime(3);
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

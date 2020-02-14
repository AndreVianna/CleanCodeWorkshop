using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using XPenC.BusinessLogic.Contracts;
using XPenC.BusinessLogic.Contracts.Models;
using XPenC.DataAccess.Contracts;
using XPenC.WebApp.Controllers.Filters;
using XPenC.WebApp.Controllers.ViewModels;
using static XPenC.WebApp.Controllers.ViewModels.ConversionHelper;

namespace XPenC.WebApp.Controllers.Controllers
{
    [TypeFilter(typeof(GeneralExceptionFilter))]
    public class ExpenseReportsController : Controller
    {
        private readonly IDataContext _dataContext;
        private readonly IExpenseReportOperations _expenseReportOperations;

        public static string AddActionName => "Add";
        public static string RemoveActionName => "Remove";
        public static string FinishActionName => "Finish";
        public static string SaveActionName => "Save";

        public ExpenseReportsController(IDataContext dataContext, IExpenseReportOperations expenseReportOperations)
        {
            _dataContext = dataContext;
            _expenseReportOperations = expenseReportOperations;
        }

        // GET: ExpenseReports
        public IActionResult Index()
        {
            var expenseReportList = _expenseReportOperations.GetList();
            
            var result = expenseReportList.Select(ToExpenseReportListItem).ToArray();
            
            return View(result);
        }

        // GET: ExpenseReports/Details/5
        public IActionResult Details(int? id)
        {
            if (id == null)
            {
                return BadRequest();
            }

            var existingReport = _expenseReportOperations.Find(id.Value);
            if (existingReport == null)
            {
                return NotFound();
            }

            var result = ToExpenseReportDetails(existingReport);

            return View(result);
        }

        // GET: ExpenseReports/Create
        public IActionResult Create()
        {
            var expenseReport = _expenseReportOperations.CreateWithDefaults();
            
            _expenseReportOperations.Add(expenseReport);
            
            _dataContext.CommitChanges();

            return RedirectToAction(nameof(Update), new { expenseReport.Id });
        }

        // GET: ExpenseReports/Update/5
        public IActionResult Update(int? id)
        {
            if (id == null)
            {
                return BadRequest();
            }

            var existingReport = _expenseReportOperations.Find(id.Value);
            if (existingReport == null)
            {
                return NotFound();
            }

            var result = ToExpenseReportUpdate(existingReport);
            PrepareExpenseTypeDropdownList();
            return View(result);
        }

        // POST: ExpenseReports/Update/5?action=Add
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Update(int? id, string action, ExpenseReportUpdate input)
        {
            if (id == null)
            {
                return BadRequest();
            }

            if (!IsValidAction(action))
            {
                return BadRequest("The action is not valid.");
            }

            if (input == null || input.Id != id)
            {
                return BadRequest("The update request data is not valid.");
            }

            var existingReport = _expenseReportOperations.Find(id.Value);
            if (existingReport == null)
            {
                return NotFound();
            }

            ValidateUpdateOperation(action, input);
            if (!ModelState.IsValid)
            {
                UpdateExpenseReportUpdate(input, existingReport);
                PrepareExpenseTypeDropdownList();
                return View(input);
            }

            ExecuteUpdateAction(action, existingReport, input);

            _dataContext.CommitChanges();

            if (action == "Finish")
            {
                return RedirectToAction(nameof(Index));
            }

            return RedirectToAction(nameof(Update), new {id});
        }

        private static bool IsValidAction(string action)
        {
            return !string.IsNullOrWhiteSpace(action) && (action == AddActionName || action == FinishActionName || action == SaveActionName || IsRemoveAction(action));
        }

        // GET: ExpenseReports/Delete/5
        public IActionResult Delete(int? id)
        {
            if (id == null)
            {
                return BadRequest();
            }

            var existingReport = _expenseReportOperations.Find(id.Value);
            if (existingReport == null)
            {
                return NotFound();
            }

            var result = ToExpenseReportDetails(existingReport);
            return View(result);
        }

        // POST: ExpenseReports/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ExecuteDelete(int id)
        {
            _expenseReportOperations.Delete(id);

            _dataContext.CommitChanges();
            
            return RedirectToAction(nameof(Index));
        }

        private void ExecuteUpdateAction(string action, ExpenseReport originalValue, ExpenseReportUpdate input)
        {
            if (IsRemoveAction(action))
            {
                _expenseReportOperations.RemoveItem(originalValue, GetItemNumberFromAction(action));
            }
            else if (action == "Add")
            {
                _expenseReportOperations.AddItem(originalValue, ToExpenseReportItem(input));
            }
            originalValue.Client = input.Client;
            _expenseReportOperations.Update(originalValue);
        }

        private void ValidateUpdateOperation(string action, ExpenseReportUpdate input)
        {
            if (string.IsNullOrWhiteSpace(input.Client))
            {
                ModelState.AddModelError("Client", "The client field is required.");
            }

            if (action != "Add")
            {
                return;
            }

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

            if (string.IsNullOrWhiteSpace(input.NewItem.ExpenseType))
            {
                ModelState.AddModelError("NewItem.ExpenseType", "The new expense type is required.");
            }

            if (!Enum.TryParse<ExpenseType>(input.NewItem.ExpenseType, out _))
            {
                ModelState.AddModelError("NewItem.ExpenseType", $"'{input.NewItem.ExpenseType}' is not a valid expense type.");
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

        public static string GetExpenseTypeDisplayName(ExpenseType expenseType)
        {
            return expenseType switch
            {
                ExpenseType.Office => "Office",
                ExpenseType.Meal => "Meal",
                ExpenseType.HotelLodging => "Lodging (Hotel)",
                ExpenseType.OtherLodging => "Lodging (Other)",
                ExpenseType.LandTransportation => "Transportation (Land)",
                ExpenseType.AirTransportation => "Transportation (Air)",
                _ => "Other",
            };
        }

        private void PrepareExpenseTypeDropdownList()
        {
            ViewData["ExpenseTypes"] = new List<SelectListItem>
            {
                new SelectListItem(GetExpenseTypeDisplayName(ExpenseType.Office), ExpenseType.Office.ToString()),
                new SelectListItem(GetExpenseTypeDisplayName(ExpenseType.Meal), ExpenseType.Meal.ToString()),
                new SelectListItem(GetExpenseTypeDisplayName(ExpenseType.HotelLodging), ExpenseType.HotelLodging.ToString()),
                new SelectListItem(GetExpenseTypeDisplayName(ExpenseType.OtherLodging), ExpenseType.OtherLodging.ToString()),
                new SelectListItem(GetExpenseTypeDisplayName(ExpenseType.LandTransportation), ExpenseType.LandTransportation.ToString()),
                new SelectListItem(GetExpenseTypeDisplayName(ExpenseType.AirTransportation), ExpenseType.AirTransportation.ToString()),
                new SelectListItem(GetExpenseTypeDisplayName(ExpenseType.Other), ExpenseType.Other.ToString()),
            };
        }
    }
}

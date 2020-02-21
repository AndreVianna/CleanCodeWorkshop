using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using XPenC.BusinessLogic.Contracts;
using XPenC.BusinessLogic.Contracts.Models;
using XPenC.DataAccess.Contracts;
using XPenC.WebApp.Filters;
using XPenC.WebApp.Models;
using static XPenC.WebApp.Models.ConversionHelper;
using ExpenseType = XPenC.BusinessLogic.Contracts.Models.ExpenseType;
using ViewExpenseType = XPenC.WebApp.Models.ExpenseType;

namespace XPenC.WebApp.Controllers
{
    [TypeFilter(typeof(GeneralExceptionFilter))]
    public class ExpenseReportsController : Controller
    {
        private readonly IDataContext _dataContext;
        private readonly IExpenseReportOperations _expenseReportOperations;

        private const string REMOVE_ACTION_NAME = "Remove";
        private const string ADD_ACTION_NAME = "Add";
        private const string FINISH_ACTION_NAME = "Finish";
        private const string SAVE_ACTION_NAME = "Save";

        internal static string RemoveActionName => REMOVE_ACTION_NAME;

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
            PrepareUpdateViewViewData();
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
                PrepareUpdateViewViewData();
                return View(input);
            }

            ExecuteUpdateAction(action, existingReport, input);

            _dataContext.CommitChanges();

            if (action == FINISH_ACTION_NAME)
            {
                return RedirectToAction(nameof(Index));
            }

            return RedirectToAction(nameof(Update), new {id});
        }

        private static bool IsValidAction(string action)
        {
            return !string.IsNullOrWhiteSpace(action) && (action == ADD_ACTION_NAME || action == FINISH_ACTION_NAME || action == SAVE_ACTION_NAME || IsRemoveAction(action));
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
            else if (action == ADD_ACTION_NAME)
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

            if (action != ADD_ACTION_NAME)
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

            if (input.NewItem.ExpenseType == null)
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

        public static ViewExpenseType GetExpenseTypeDisplayName(ExpenseType expenseType)
        {
            return expenseType switch
            {
                ExpenseType.Office => ViewExpenseType.Office,
                ExpenseType.Meal => ViewExpenseType.Meal,
                ExpenseType.HotelLodging => ViewExpenseType.HotelLodging,
                ExpenseType.OtherLodging => ViewExpenseType.OtherLodging,
                ExpenseType.LandTransportation => ViewExpenseType.LandTransportation,
                ExpenseType.AirTransportation => ViewExpenseType.AirTransportation,
                _ => ViewExpenseType.Other,
            };
        }

        private void PrepareUpdateViewViewData()
        {
            ViewData["AddAction"] = ADD_ACTION_NAME;
            ViewData["RemoveAction"] = REMOVE_ACTION_NAME;
            ViewData["SaveAction"] = SAVE_ACTION_NAME;
            ViewData["FinishAction"] = FINISH_ACTION_NAME;
        }
    }
}

using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using XPenC.BusinessLogic.Contracts;
using XPenC.BusinessLogic.Contracts.Exceptions;
using XPenC.DataAccess.Contracts;
using XPenC.WebApp.Filters;
using XPenC.WebApp.Models;
using static XPenC.WebApp.Models.ConversionHelper;
using ExpenseType = XPenC.BusinessLogic.Contracts.Models.ExpenseType;
using ViewExpenseType = XPenC.WebApp.Models.ExpenseType;

namespace XPenC.WebApp.Controllers
{
    [Route("ExpenseReports")]
    [TypeFilter(typeof(GeneralExceptionFilter))]
    public class ExpenseReportsController : Controller
    {
        private readonly IDataContext _dataContext;
        private readonly IExpenseReportOperations _expenseReportOperations;
        private readonly IStringLocalizer<ExpenseReportsController> _strings;

        private const string FINISH_ACTION_NAME = "Finish";
        private const string SAVE_ACTION_NAME = "Save";

        public ExpenseReportsController(IDataContext dataContext, IExpenseReportOperations expenseReportOperations, IStringLocalizer<ExpenseReportsController> strings)
        {
            _dataContext = dataContext;
            _expenseReportOperations = expenseReportOperations;
            _strings = strings;
        }

        [HttpGet("")]
        public IActionResult Index()
        {
            var expenseReportList = _expenseReportOperations.GetList();
            
            var result = expenseReportList.Select(ToExpenseReportListItem).ToArray();
            
            return View(result);
        }

        [HttpGet("Details/{id}")]
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

        [HttpGet("Create")]
        public IActionResult Create()
        {
            var expenseReport = _expenseReportOperations.CreateWithDefaults();

            return View(ToExpenseReportUpdate(expenseReport));
        }

        [HttpPost("Create")]
        [ValidateAntiForgeryToken]
        public IActionResult Create(string action, ExpenseReportUpdate input)
        {
            if (input == null)
            {
                return BadRequest("The create request data is not valid.");
            }

            if (!ModelState.IsValid)
            {
                return View(input);
            }

            try
            {
                var newExpenseReport = _expenseReportOperations.CreateWithDefaults();

                UpdateExpenseReport(newExpenseReport, input);

                _expenseReportOperations.Add(newExpenseReport);

                _dataContext.CommitChanges();

                return action == FINISH_ACTION_NAME
                    ? RedirectToAction(nameof(Index))
                    : RedirectToAction(nameof(Update), new { newExpenseReport.Id });
            }
            catch (ValidationException ex)
            {
                foreach (var validationError in ex.Errors)
                {
                    ModelState.AddModelError(validationError.Source, _strings[validationError.Message]);
                }
                PrepareUpdateViewViewData();
                return View(input);
            }
        }

        [HttpGet("Update/{id}")]
        public IActionResult Update(int id)
        {
            var existingReport = _expenseReportOperations.Find(id);
            if (existingReport == null)
            {
                return NotFound();
            }

            var result = ToExpenseReportUpdate(existingReport);
            PrepareUpdateViewViewData();
            return View(result);
        }

        [HttpPost("Update/{id}")]
        [ValidateAntiForgeryToken]
        public IActionResult Update(int id, string action, ExpenseReportUpdate input)
        {
            if (!IsValidAction(action))
            {
                return BadRequest("The action is not valid.");
            }

            if (input == null || input.Id != id)
            {
                return BadRequest("The update request data is not valid.");
            }

            var existingReport = _expenseReportOperations.Find(id);
            if (existingReport == null)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return View(input);
            }

            try
            {
                UpdateExpenseReport(existingReport, input);

                _expenseReportOperations.Update(existingReport);

                _dataContext.CommitChanges();

                return action == FINISH_ACTION_NAME
                    ? RedirectToAction(nameof(Index))
                    : RedirectToAction(nameof(Update), new {id});
            }
            catch (ValidationException ex)
            {
                foreach (var validationError in ex.Errors)
                {
                    ModelState.AddModelError(validationError.Source, _strings[validationError.Message]);
                }
                UpdateExpenseReportUpdate(input, existingReport);
                PrepareUpdateViewViewData();
                return View(input);
            }
        }

        private static bool IsValidAction(string action)
        {
            return !string.IsNullOrWhiteSpace(action) && (action == FINISH_ACTION_NAME || action == SAVE_ACTION_NAME);
        }

        // GET: ExpenseReports/Delete/5
        [HttpGet("Delete/{id}")]
        public IActionResult Delete(int id)
        {
            var existingReport = _expenseReportOperations.Find(id);
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
            ViewData["SaveAction"] = SAVE_ACTION_NAME;
            ViewData["FinishAction"] = FINISH_ACTION_NAME;
        }
    }
}

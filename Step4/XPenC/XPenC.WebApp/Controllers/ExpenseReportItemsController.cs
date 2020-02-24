using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using XPenC.BusinessLogic.Contracts;
using XPenC.BusinessLogic.Contracts.Exceptions;
using XPenC.DataAccess.Contracts;
using XPenC.WebApp.Filters;
using XPenC.WebApp.Models.ExpenseReports;
using static XPenC.WebApp.Models.ExpenseReports.ConversionHelper;

namespace XPenC.WebApp.Controllers
{
    [Route("ExpenseReports/{id}/Items")]
    [TypeFilter(typeof(GeneralExceptionFilter))]
    public class ExpenseReportItemsController : Controller
    {
        private readonly IDataContext _dataContext;
        private readonly IExpenseReportItemOperations _expenseReportItemOperations;
        private readonly IExpenseReportOperations _expenseReportOperations;
        private readonly IStringLocalizer<ExpenseReportItemsController> _strings;

        public ExpenseReportItemsController(IDataContext dataContext, IExpenseReportOperations expenseReportOperations, IExpenseReportItemOperations expenseReportItemOperations, IStringLocalizer<ExpenseReportItemsController> strings)
        {
            _dataContext = dataContext;
            _expenseReportOperations = expenseReportOperations;
            _expenseReportItemOperations = expenseReportItemOperations;
            _strings = strings;
        }

        // GET: ExpenseReports/Update/5/Items/Add
        [HttpGet("Create")]
        public IActionResult Create(int id)
        {
            var existingReport = _expenseReportOperations.Find(id);
            if (existingReport == null)
            {
                return NotFound();
            }

            var result = ToExpenseReportItemUpdate(existingReport);
            return View(result);
        }

        // POST: ExpenseReports/Update/5/Items
        [HttpPost("Create")]
        [ValidateAntiForgeryToken]
        public IActionResult Create(int id, ExpenseReportItemUpdate input)
        {
            if (input == null)
            {
                return BadRequest("The add item request data is not valid.");
            }

            if (!ModelState.IsValid)
            {
                return View(input);
            }

            try
            {
                _expenseReportItemOperations.Add(ToExpenseReportItem(input));

                _dataContext.CommitChanges();

                return RedirectToAction("Update", "ExpenseReports", new {id});
            }
            catch (ValidationException ex)
            {
                foreach (var validationError in ex.Errors)
                {
                    ModelState.AddModelError(validationError.Source, _strings[validationError.Message]);
                }
                var existingReport = _expenseReportOperations.Find(id);
                UpdateExpenseReportItemUpdate(input, existingReport);
                return View(input);
            }
        }

        [HttpGet("Delete/{number}")]
        public IActionResult Delete(int id, int number)
        {
            var existingReport = _expenseReportOperations.Find(id);
            if (existingReport == null || existingReport.Items.All(i => i.ItemNumber != number))
            {
                return NotFound();
            }

            var result = ToExpenseReportDetails(existingReport);
            ViewData["Number"] = number;
            return View(result);
        }

        [HttpPost("Delete/{number}")]
        [ValidateAntiForgeryToken]
        public IActionResult ExecuteDelete(int id, int number)
        {
            _expenseReportItemOperations.Delete(id, number);

            _dataContext.CommitChanges();

            return RedirectToAction("Update", "ExpenseReports", new { id });
        }
    }
}
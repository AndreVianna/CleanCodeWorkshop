using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using XPenC.BusinessLogic.Contracts;
using XPenC.BusinessLogic.Contracts.Models;
using XPenC.DataAccess.Contracts;
using XPenC.WebApp.ViewModels;
using ConversionHelper = XPenC.WebApp.ViewModels.ConversionHelper;

namespace XPenC.WebApp.Controllers
{
    public class ExpenseReportsController : Controller
    {
        private readonly IDataContext _dataContext;
        private readonly IExpenseReportOperations _expenseReportOperations;

        public ExpenseReportsController(IDataContext dataContext, IExpenseReportOperations expenseReportOperations)
        {
            _dataContext = dataContext;
            _expenseReportOperations = expenseReportOperations;
        }

        // GET: ExpenseReports
        public IActionResult Index()
        {
            var expenseReportList = _expenseReportOperations.GetList();
            var result = expenseReportList.Select(ConversionHelper.ToExpenseReportListItem);
            return View(result);
        }

        // GET: ExpenseReports/Details/5
        public IActionResult Details(int? id)
        {
            if (id == null)
                return NotFound();

            var existingReport = _expenseReportOperations.Find(id.Value);
            var result = ConversionHelper.ToExpenseReportDetails(existingReport);

            if (result == null)
                return NotFound();

            return View(result);
        }

        // GET: ExpenseReports/Create
        public IActionResult Create()
        {
            try
            {
                var expenseReport = _expenseReportOperations.CreateWithDefaults();
                _expenseReportOperations.Add(expenseReport);
                _dataContext.CommitChanges();

                return RedirectToAction(nameof(Update), new { expenseReport.Id });
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

            var existingReport = _expenseReportOperations.Find(id.Value);
            var result = ConversionHelper.ToExpenseReportUpdateInput(existingReport);

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
            try
            {
                if (id == null)
                    return NotFound();

                if (input == null || input.Id != id)
                    return BadRequest("The update request data is not valid.");

                var existingReport = _expenseReportOperations.Find(id.Value);

                if (existingReport == null)
                    return NotFound();

                ValidateUpdateOperation(action, input);
                if (!ModelState.IsValid)
                {
                    input = ConversionHelper.RestoreInputItems(existingReport, input);
                    PrepareExpenseTypeDropdownList();
                    return View(input);
                }

                ExecuteUpdateAction(action, existingReport, input);

                _dataContext.CommitChanges();

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

            var existingReport = _expenseReportOperations.Find(id.Value);

            if (existingReport == null)
                return NotFound();

            var result = ConversionHelper.ToExpenseReportDetails(existingReport);
            return View(result);
        }

        // POST: ExpenseReports/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ExecuteDelete(int id)
        {
            try
            {
                _expenseReportOperations.Delete(id);
                _dataContext.CommitChanges();
            }
            catch (Exception ex)
            {
                return RedirectToAction("Error", "Home", new { message = ex.Message });
            }
            return RedirectToAction(nameof(Index));
        }

        public void ExecuteUpdateAction(string action, ExpenseReport originalValue, ExpenseReportUpdateInput input)
        {
            var updateActions = new List<Action>();
            if (input.Client != originalValue.Client)
            {
                originalValue.Client = input.Client;
                updateActions.Add(() => _expenseReportOperations.Update(originalValue));
            }

            if (action.StartsWith("Remove"))
            {
                var itemNumber = Convert.ToInt32(action.Replace("Remove", ""));
                updateActions.Add(() => _expenseReportOperations.RemoveItem(originalValue, itemNumber));
            }

            if (action == "Add")
            {
                updateActions.Add(() => _expenseReportOperations.AddItem(originalValue, ConversionHelper.ToExpenseReportItem(input)));
            }

            if (updateActions.Count > 0)
            {
                updateActions.Add(() => _expenseReportOperations.UpdateLastModificationDate(originalValue));
            }

            foreach (var updateAction in updateActions)
            {
                updateAction();
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
    }
}

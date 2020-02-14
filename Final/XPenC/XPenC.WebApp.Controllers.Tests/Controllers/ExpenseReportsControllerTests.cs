using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using XPenC.WebApp.Controllers.Controllers;
using XPenC.WebApp.Controllers.Tests.TestDoubles;
using XPenC.WebApp.Controllers.ViewModels;
using Xunit;
using static XPenC.WebApp.Controllers.Controllers.ExpenseReportsController;
using static XPenC.WebApp.Controllers.Tests.TestDoubles.FakeExpenseReportOperations;

namespace XPenC.WebApp.Controllers.Tests.Controllers
{
    public class ExpenseReportsControllerTests
    {
        private readonly ExpenseReportsController _controller;

        public ExpenseReportsControllerTests()
        {
            var dataContext = new StubDataContext();
            var expenseReportOperations = new FakeExpenseReportOperations();
            _controller = new ExpenseReportsController(dataContext, expenseReportOperations);
        }

        [Fact]
        public void ExpenseReportsController_Index_ShouldPass()
        {
            var result = _controller.Index();

            var viewResult = Assert.IsType<ViewResult>(result);

            Assert.Null(viewResult.ViewName);
            var model = Assert.IsAssignableFrom<IEnumerable<ExpenseReportListItem>>(viewResult.Model).ToList();
            Assert.Equal(ExistingReport1.Id, model[0].Id);
            Assert.Equal(ExistingReport1.Client, model[0].Client);
            Assert.Equal(ExistingReport1.CreatedOn, model[0].CreatedOn);
            Assert.Equal(ExistingReport1.ModifiedOn, model[0].ModifiedOn);
        }

        [Fact]
        public void ExpenseReportsController_Details_WithNullId_ShouldPass()
        {
            var result = _controller.Details(null);

            Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public void ExpenseReportsController_Details_WithInvalidId_ShouldPass()
        {
            var result = _controller.Details(32);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public void ExpenseReportsController_Details_WithValidId_ShouldPass()
        {
            var result = _controller.Details(ExistingReport1.Id);

            var viewResult = Assert.IsType<ViewResult>(result);
            
            Assert.Null(viewResult.ViewName);
            var model = Assert.IsType<ExpenseReportDetails>(viewResult.Model);
            Assert.Equal(ExistingReport1.Id, model.Id);
            Assert.Equal(ExistingReport1.MealTotal, model.MealTotal);
            Assert.Equal(ExistingReport1.Total, model.Total);
            Assert.Equal(ExistingReport1.Client, model.Client);
            Assert.Equal(ExistingReport1.CreatedOn, model.CreatedOn);
            Assert.Equal(ExistingReport1.ModifiedOn, model.ModifiedOn);
            Assert.Equal(ExistingReport1.Items.Count, model.Items.Count);
            var existingItems = ExistingReport1.Items.ToList();
            for (var i = 0; i < existingItems.Count; i++)
            {
                Assert.Equal(existingItems[i].ExpenseReportId, model.Id);
                Assert.Equal(existingItems[i].ItemNumber, model.Items[i].Number);
                Assert.Equal(existingItems[i].Description, model.Items[i].Description);
                Assert.Equal(existingItems[i].Date, model.Items[i].Date);
                Assert.Equal(existingItems[i].Value, model.Items[i].Value);
                Assert.Equal(existingItems[i].IsAboveMaximum, model.Items[i].IsAboveMaximum);
                Assert.Equal(GetExpenseTypeDisplayName(existingItems[i].ExpenseType), model.Items[i].ExpenseType);
            }
        }

        [Fact]
        public void ExpenseReportsController_Create_ShouldPass()
        {
            var result = _controller.Create();

            var viewResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Update", viewResult.ActionName);
        }

        [Fact]
        public void ExpenseReportsController_Update_Get_WithNullId_ShouldPass()
        {
            var result = _controller.Update(null);

            Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public void ExpenseReportsController_Update_Get_WithInvalidId_ShouldPass()
        {
            var result = _controller.Update(32);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public void ExpenseReportsController_Update_Get_WithValidId_ShouldPass()
        {
            var result = _controller.Update(1);

            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Null(viewResult.ViewName);
            var model = Assert.IsType<ExpenseReportUpdate>(viewResult.Model);
            Assert.Equal(ExistingReport1.Id, model.Id);
            Assert.Equal(ExistingReport1.Client, model.Client);
            Assert.Equal(AddActionName, model.AddActionName);
            Assert.Equal(SaveActionName, model.SaveActionName);
            Assert.Equal(FinishActionName, model.FinishActionName);
            Assert.Equal(ExistingReport1.Items.Count, model.DisplayItems.Count);
            var existingItems = ExistingReport1.Items.ToList();
            Assert.Equal(existingItems[0].ItemNumber, model.DisplayItems[0].Number);
            Assert.Equal(existingItems[0].Description, model.DisplayItems[0].Description);
            Assert.Equal(existingItems[0].Date, model.DisplayItems[0].Date);
            Assert.Equal(existingItems[0].Value, model.DisplayItems[0].Value);
            Assert.Equal(existingItems[0].IsAboveMaximum, model.DisplayItems[0].IsAboveMaximum);
            Assert.Equal(existingItems[0].ExpenseType.ToString(), model.DisplayItems[0].ExpenseType);
            Assert.Equal($"{RemoveActionName}{model.DisplayItems[0].Number}", model.DisplayItems[0].RemoveActionName);
            Assert.Null(model.NewItem.Description);
            Assert.NotNull(model.NewItem.Date);
            Assert.Equal(0, model.NewItem.Value);
            Assert.Null(model.NewItem.ExpenseType);
        }

        [Fact]
        public void ExpenseReportsController_Update_Post_WithNullId_ShouldPass()
        {
            var result = _controller.Update(null, null, null);

            Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public void ExpenseReportsController_Update_Post_WithNullAction_ShouldPass()
        {
            var result = _controller.Update(32, null, null);

            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public void ExpenseReportsController_Update_Post_WithEmptyAction_ShouldPass()
        {
            var result = _controller.Update(32, " ", null);

            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public void ExpenseReportsController_Update_Post_WithInvalidAction_ShouldPass()
        {
            var result = _controller.Update(32, "Invalid", null);

            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public void ExpenseReportsController_Update_Post_WithNullInput_ShouldPass()
        {
            var result = _controller.Update(32, "Save", null);

            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public void ExpenseReportsController_Update_Post_WithInputWithInvalidId_ShouldPass()
        {
            var input = new ExpenseReportUpdate { Id = 15 };
            var result = _controller.Update(32, "Save", input);

            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public void ExpenseReportsController_Update_Post_WithNonExistingId_ShouldPass()
        {
            var input = new ExpenseReportUpdate { Id = 32 };
            var result = _controller.Update(32, "Save", input);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public void ExpenseReportsController_Update_Post_WithEmptyClient_ShouldPass()
        {
            var input = new ExpenseReportUpdate { Id = 1 };
            var result = _controller.Update(1, "Save", input);

            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Null(viewResult.ViewName);
        }

        [Fact]
        public void ExpenseReportsController_Update_Post_ForAdd_WithNullNewItemDate_ShouldPass()
        {
            var newItem = new ExpenseReportItemUpdate { Date = null };
            var input = new ExpenseReportUpdate { Id = 1, Client = "Some Client", NewItem = newItem };
            var result = _controller.Update(1, "Add", input);

            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Null(viewResult.ViewName);
        }

        [Fact]
        public void ExpenseReportsController_Update_Post_ForAdd_WithNewItemDateInTheFuture_ShouldPass()
        {
            var newItem = new ExpenseReportItemUpdate { Date = DateTime.Now.AddHours(1)};
            var input = new ExpenseReportUpdate { Id = 1, Client = "Some Client", NewItem = newItem };
            var result = _controller.Update(1, "Add", input);

            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Null(viewResult.ViewName);
        }

        [Fact]
        public void ExpenseReportsController_Update_Post_ForAdd_WithNullExpenseType_ShouldPass()
        {
            var newItem = new ExpenseReportItemUpdate { Date = DateTime.Now.AddDays(-1), ExpenseType = null };
            var input = new ExpenseReportUpdate { Id = 1, Client = "Some Client", NewItem = newItem };
            var result = _controller.Update(1, "Add", input);

            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Null(viewResult.ViewName);
        }

        [Fact]
        public void ExpenseReportsController_Update_Post_ForAdd_WithEmptyExpenseType_ShouldPass()
        {
            var newItem = new ExpenseReportItemUpdate { Date = DateTime.Now.AddDays(-1), ExpenseType = " " };
            var input = new ExpenseReportUpdate { Id = 1, Client = "Some Client", NewItem = newItem };
            var result = _controller.Update(1, "Add", input);

            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Null(viewResult.ViewName);
        }

        [Fact]
        public void ExpenseReportsController_Update_Post_ForAdd_WithInvalidExpenseType_ShouldPass()
        {
            var newItem = new ExpenseReportItemUpdate { Date = DateTime.Now.AddDays(-1), ExpenseType = "Invalid" };
            var input = new ExpenseReportUpdate { Id = 1, Client = "Some Client", NewItem = newItem };
            var result = _controller.Update(1, "Add", input);

            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Null(viewResult.ViewName);
        }

        [Fact]
        public void ExpenseReportsController_Update_Post_ForAdd_WithNullValue_ShouldPass()
        {
            var newItem = new ExpenseReportItemUpdate { Date = DateTime.Now.AddDays(-1), ExpenseType = "Meal", Value = null };
            var input = new ExpenseReportUpdate { Id = 1, Client = "Some Client", NewItem = newItem };
            var result = _controller.Update(1, "Add", input);

            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Null(viewResult.ViewName);
        }

        [Fact]
        public void ExpenseReportsController_Update_Post_ForAdd_WithNegativeValue_ShouldPass()
        {
            var newItem = new ExpenseReportItemUpdate { Date = DateTime.Now.AddDays(-1), ExpenseType = "Meal", Value = -1 };
            var input = new ExpenseReportUpdate { Id = 1, Client = "Some Client", NewItem = newItem };
            var result = _controller.Update(1, "Add", input);

            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Null(viewResult.ViewName);
        }

        [Fact]
        public void ExpenseReportsController_Update_Post_ForAdd_ShouldPass()
        {
            var newItem = new ExpenseReportItemUpdate { Date = DateTime.Now.AddDays(-1), ExpenseType = "Meal", Value = 10, Description = "Some Description" };
            var input = new ExpenseReportUpdate { Id = 1, Client = "Some Client", NewItem = newItem };
            var result = _controller.Update(1, "Add", input);

            var viewResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Update", viewResult.ActionName);
        }

        [Fact]
        public void ExpenseReportsController_Update_Post_ForRemove_ShouldPass()
        {
            var input = new ExpenseReportUpdate { Id = 1, Client = "Some Client" };
            var result = _controller.Update(1, "Remove1", input);

            var viewResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Update", viewResult.ActionName);
        }

        [Fact]
        public void ExpenseReportsController_Update_Post_ForRemove_WithInvalidItemNumber_ShouldPass()
        {
            var input = new ExpenseReportUpdate { Id = 1, Client = "Some Client" };
            var result = _controller.Update(1, "Remove32", input);

            var viewResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Update", viewResult.ActionName);
        }

        [Fact]
        public void ExpenseReportsController_Update_Post_ForSave_ShouldPass()
        {
            var input = new ExpenseReportUpdate { Id = 1, Client = "Some Client" };
            var result = _controller.Update(1, "Save", input);

            var viewResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Update", viewResult.ActionName);
        }

        [Fact]
        public void ExpenseReportsController_Update_Post_ForFinish_ShouldPass()
        {
            var input = new ExpenseReportUpdate { Id = 1, Client = "Some Client" };
            var result = _controller.Update(1, "Finish", input);

            var viewResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", viewResult.ActionName);
        }


        [Fact]
        public void ExpenseReportsController_Delete_WithNullId_ShouldPass()
        {
            var result = _controller.Delete(null);

            Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public void ExpenseReportsController_Delete_WithInvalidId_ShouldPass()
        {
            var result = _controller.Delete(32);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public void ExpenseReportsController_Delete_WithValidId_ShouldPass()
        {
            var result = _controller.Delete(1);

            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Null(viewResult.ViewName);
        }


        [Fact]
        public void ExpenseReportsController_ExecuteDelete_ShouldPass()
        {
            var result = _controller.ExecuteDelete(1);

            var viewResult = Assert.IsType<RedirectToActionResult>(result);
            
            Assert.Equal("Index", viewResult.ActionName);
        }
    }
}
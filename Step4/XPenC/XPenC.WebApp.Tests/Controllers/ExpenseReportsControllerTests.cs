using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using XPenC.BusinessLogic.Contracts.Exceptions;
using XPenC.WebApp.Controllers;
using XPenC.WebApp.Models.ExpenseReports;
using XPenC.WebApp.Tests.TestDoubles;
using Xunit;
using static XPenC.WebApp.Controllers.ExpenseReportsController;
using static XPenC.WebApp.Tests.TestDoubles.FakeExpenseReportOperations;

namespace XPenC.WebApp.Tests.Controllers
{
    public class ExpenseReportsControllerTests
    {
        private readonly ExpenseReportsController _controller;
        private readonly FakeExpenseReportOperations _expenseReportOperations = new FakeExpenseReportOperations();

        public ExpenseReportsControllerTests()
        {
            var dataContext = new StubDataContext();
            _controller = new ExpenseReportsController(dataContext, _expenseReportOperations, new FakeStringLocalizer<ExpenseReportsController>());
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

            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Null(viewResult.ViewName);
        }

        [Fact]
        public void ExpenseReportsController_Create_Post_WithNullAction_ShouldPass()
        {
            var result = _controller.Create(null, null);

            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public void ExpenseReportsController_Create_Post_WithEmptyAction_ShouldPass()
        {
            var result = _controller.Create(" ", null);

            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public void ExpenseReportsController_Create_Post_WithInvalidAction_ShouldPass()
        {
            var result = _controller.Create("Invalid", null);

            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public void ExpenseReportsController_Create_Post_WithNullInput_ShouldPass()
        {
            var result = _controller.Create( "Save", null);

            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public void ExpenseReportsController_Create_Post_WithModelError_ShouldPass()
        {
            var input = new ExpenseReportUpdate { Id = 1, Client = "Some Client" };
            _controller.ModelState.AddModelError("", "Some error");
            var result = _controller.Create("Save", input);

            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Null(viewResult.ViewName);
        }

        [Fact]
        public void ExpenseReportsController_Create_Post_WithValidationError_ShouldPass()
        {
            var input = new ExpenseReportUpdate { Id = 1 };
            var expectedError = new ValidationError
            {
                Source = "Client",
                Message = "The 'Client' field is required.",
            };
            _expenseReportOperations.ExpectedAddBehavior = () => throw new ValidationException("Add", new[] { expectedError });
            var result = _controller.Create("Save", input);

            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Null(viewResult.ViewName);
        }

        [Fact]
        public void ExpenseReportsController_Create_Post_ForSave_ShouldPass()
        {
            var input = new ExpenseReportUpdate { Id = 1, Client = "Some Client" };
            var result = _controller.Create("Save", input);

            var viewResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Update", viewResult.ActionName);
        }

        [Fact]
        public void ExpenseReportsController_Create_Post_ForFinish_ShouldPass()
        {
            var input = new ExpenseReportUpdate { Id = 1, Client = "Some Client" };
            var result = _controller.Create( "Finish", input);

            var viewResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", viewResult.ActionName);
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
            Assert.Equal(ExistingReport1.Items.Count, model.Items.Count);
            var existingItems = ExistingReport1.Items.ToList();
            Assert.Equal(existingItems[0].ItemNumber, model.Items[0].Number);
            Assert.Equal(existingItems[0].Description, model.Items[0].Description);
            Assert.Equal(existingItems[0].Date, model.Items[0].Date);
            Assert.Equal(existingItems[0].Value, model.Items[0].Value);
            Assert.Equal(existingItems[0].IsAboveMaximum, model.Items[0].IsAboveMaximum);
            Assert.Equal(existingItems[0].ExpenseType.ToString(), model.Items[0].ExpenseType.ToString());
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
        public void ExpenseReportsController_Update_Post_WithModelError_ShouldPass()
        {
            var input = new ExpenseReportUpdate { Id = 1, Client = "Some Client" };
            _controller.ModelState.AddModelError("", "Some error");
            var result = _controller.Update(1, "Save", input);

            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Null(viewResult.ViewName);
        }

        [Fact]
        public void ExpenseReportsController_Update_Post_WithValidationError_ShouldPass()
        {
            var input = new ExpenseReportUpdate { Id = 1 };
            var expectedError = new ValidationError
            {
                Source = "Client",
                Message = "The 'Client' field is required.",
            };
            _expenseReportOperations.ExpectedUpdateBehavior = () => throw new ValidationException("Update", new [] { expectedError });
            var result = _controller.Update(1, "Save", input);

            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Null(viewResult.ViewName);
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
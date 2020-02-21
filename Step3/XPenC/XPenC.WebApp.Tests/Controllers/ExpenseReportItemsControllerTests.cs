using System;
using Microsoft.AspNetCore.Mvc;
using XPenC.BusinessLogic.Exceptions;
using XPenC.BusinessLogic.Validation;
using XPenC.WebApp.Controllers;
using XPenC.WebApp.Models;
using XPenC.WebApp.Tests.TestDoubles;
using Xunit;

namespace XPenC.WebApp.Tests.Controllers
{
    public class ExpenseReportItemsControllerTests
    {
        private readonly ExpenseReportItemsController _controller;
        private readonly FakeExpenseReportItemOperations _expenseReportItemOperations = new FakeExpenseReportItemOperations();

        public ExpenseReportItemsControllerTests()
        {
            var dataContext = new StubDataContext();
            var expenseReportOperations = new FakeExpenseReportOperations();
            _controller = new ExpenseReportItemsController(dataContext, expenseReportOperations, _expenseReportItemOperations, new FakeStringLocalizer<ExpenseReportItemsController>());
        }

        [Fact]
        public void ExpenseReportItemsController_Create_ShouldPass()
        {
            var result = _controller.Create(1);

            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Null(viewResult.ViewName);
            var model = Assert.IsType<ExpenseReportItemUpdate>(viewResult.Model);
            Assert.Equal(1, model.ExpenseReportId);
            Assert.NotNull(model.ExpenseReport);
        }

        [Fact]
        public void ExpenseReportItemsController_Create_WithInvalidId_ShouldPass()
        {
            var result = _controller.Create(32);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public void ExpenseReportItemsController_Create_Post_WithNullData_ShouldPass()
        {
            var result = _controller.Create(1, null);

            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public void ExpenseReportItemsController_Create_Post_WithModelError_ShouldPass()
        {
            var input = new ExpenseReportItemUpdate { Date = DateTime.Now.AddDays(-1), ExpenseType = ExpenseType.Office, Value = 10, Description = "Some Description" };
            _controller.ModelState.AddModelError("", "Some error");
            var result = _controller.Create(1, input);

            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Null(viewResult.ViewName);
        }

        [Fact]
        public void ExpenseReportItemsController_Create_Post_WithValidationError_ShouldPass()
        {
            var input = new ExpenseReportItemUpdate { Date = DateTime.Now.AddHours(1) };
            _expenseReportItemOperations.ExpectedAddBehavior = () => throw new ValidationException("Add", new [] { new ValidationError("Date", "The expense item date must not be in the future.") });
            var result = _controller.Create(1, input);

            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Null(viewResult.ViewName);
        }

        [Fact]
        public void ExpenseReportItemsController_Create_PostOffice_ShouldPass()
        {
            var input = new ExpenseReportItemUpdate { Date = DateTime.Now.AddDays(-1), ExpenseType = ExpenseType.Office, Value = 10, Description = "Some Description" };
            var result = _controller.Create(1, input);

            var viewResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Update", viewResult.ActionName);
            Assert.Equal("ExpenseReports", viewResult.ControllerName);
            Assert.Equal(1, viewResult.RouteValues["id"]);
        }

        [Fact]
        public void ExpenseReportItemsController_Create_PostMeal_ShouldPass()
        {
            var input = new ExpenseReportItemUpdate { Date = DateTime.Now.AddDays(-1), ExpenseType = ExpenseType.Meal, Value = 10, Description = "Some Description" };
            var result = _controller.Create(1, input);

            var viewResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Update", viewResult.ActionName);
            Assert.Equal("ExpenseReports", viewResult.ControllerName);
            Assert.Equal(1, viewResult.RouteValues["id"]);
        }

        [Fact]
        public void ExpenseReportItemsController_Create_PostHotelLodging_ShouldPass()
        {
            var input = new ExpenseReportItemUpdate { Date = DateTime.Now.AddDays(-1), ExpenseType = ExpenseType.HotelLodging, Value = 10, Description = "Some Description" };
            var result = _controller.Create(1, input);

            var viewResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Update", viewResult.ActionName);
            Assert.Equal("ExpenseReports", viewResult.ControllerName);
            Assert.Equal(1, viewResult.RouteValues["id"]);
        }

        [Fact]
        public void ExpenseReportItemsController_Create_PostOtherLodging_ShouldPass()
        {
            var input = new ExpenseReportItemUpdate { Date = DateTime.Now.AddDays(-1), ExpenseType = ExpenseType.OtherLodging, Value = 10, Description = "Some Description" };
            var result = _controller.Create(1, input);

            var viewResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Update", viewResult.ActionName);
            Assert.Equal("ExpenseReports", viewResult.ControllerName);
            Assert.Equal(1, viewResult.RouteValues["id"]);
        }

        [Fact]
        public void ExpenseReportItemsController_Create_PostLandTransportation_ShouldPass()
        {
            var input = new ExpenseReportItemUpdate { Date = DateTime.Now.AddDays(-1), ExpenseType = ExpenseType.LandTransportation, Value = 10, Description = "Some Description" };
            var result = _controller.Create(1, input);

            var viewResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Update", viewResult.ActionName);
            Assert.Equal("ExpenseReports", viewResult.ControllerName);
            Assert.Equal(1, viewResult.RouteValues["id"]);
        }

        [Fact]
        public void ExpenseReportItemsController_Create_PostAirTransportation_ShouldPass()
        {
            var input = new ExpenseReportItemUpdate { Date = DateTime.Now.AddDays(-1), ExpenseType = ExpenseType.AirTransportation, Value = 10, Description = "Some Description" };
            var result = _controller.Create(1, input);

            var viewResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Update", viewResult.ActionName);
            Assert.Equal("ExpenseReports", viewResult.ControllerName);
            Assert.Equal(1, viewResult.RouteValues["id"]);
        }

        [Fact]
        public void ExpenseReportItemsController_Create_Post_ForOther_ShouldPass()
        {
            var input = new ExpenseReportItemUpdate { Date = DateTime.Now.AddDays(-1), ExpenseType = ExpenseType.Other, Value = 10, Description = "Some Description" };
            var result = _controller.Create(1, input);

            var viewResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Update", viewResult.ActionName);
            Assert.Equal("ExpenseReports", viewResult.ControllerName);
            Assert.Equal(1, viewResult.RouteValues["id"]);
        }

        [Fact]
        public void ExpenseReportItemsController_Delete_WithInvalidId_ShouldPass()
        {
            var result = _controller.Delete(32, 1);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public void ExpenseReportItemsController_Delete_WithInvalidNumber_ShouldPass()
        {
            var result = _controller.Delete(1, 32);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public void ExpenseReportItemsController_Delete_ShouldPass()
        {
            var result = _controller.Delete(1, 1);

            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Null(viewResult.ViewName);
        }


        [Fact]
        public void ExpenseReportItemsController_ExecuteDelete_ShouldPass()
        {
            var result = _controller.ExecuteDelete(1, 1);

            var viewResult = Assert.IsType<RedirectToActionResult>(result);

            Assert.Equal("Update", viewResult.ActionName);
            Assert.Equal("ExpenseReports", viewResult.ControllerName);
            Assert.Equal(1, viewResult.RouteValues["id"]);
        }
    }
}
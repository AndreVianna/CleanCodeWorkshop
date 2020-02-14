using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
using Microsoft.AspNetCore.Routing;
using XPenC.WebApp.Controllers.Filters;
using XPenC.WebApp.Controllers.Tests.TestDoubles;
using Xunit;

namespace XPenC.WebApp.Controllers.Tests.Filters
{
    public class GeneralExceptionFilterTests
    {
        [Fact]
        public void HomeController_OnException_ForDevelopment_ShouldPass()
        {
            var filter = CreateFilterFor(new FakeDevelopmentEnvironment());
            var context = CreateExceptionContext();

            filter.OnException(context);

            var viewResult = Assert.IsType<ViewResult>(context.Result);
            Assert.Equal("Error", viewResult.ViewName);
            Assert.True((bool)viewResult.ViewData["ShowException"]);
            Assert.Equal("Exception", (string)viewResult.ViewData["ExceptionType"]);
            Assert.Equal("Some message", (string)viewResult.ViewData["ExceptionMessage"]);
            Assert.NotNull((string)viewResult.ViewData["RequestId"]);
        }

        [Fact]
        public void HomeController_OnException_ForDevelopment_WithActivityId_ShouldPass()
        {
            var filter = CreateFilterFor(new FakeDevelopmentEnvironment());
            var context = CreateExceptionContext();
            var activity = new Activity("HomeController_OnException_ForDevelopment_WithActivityId_ShouldPass");
            activity.Start();
            var expectedRequestId = activity.Id;
            
            filter.OnException(context);

            activity.Stop();
            var viewResult = Assert.IsType<ViewResult>(context.Result);
            Assert.Equal("Error", viewResult.ViewName);
            Assert.True((bool)viewResult.ViewData["ShowException"]);
            Assert.Equal("Exception", (string)viewResult.ViewData["ExceptionType"]);
            Assert.Equal("Some message", (string)viewResult.ViewData["ExceptionMessage"]);
            Assert.NotNull((string)viewResult.ViewData["RequestId"]);
        }

        [Fact]
        public void HomeController_OnException_NotForDevelopment_ShouldPass()
        {
            var filter = CreateFilterFor(new FakeProductionEnvironment());
            var context = CreateExceptionContext();

            filter.OnException(context);

            var viewResult = Assert.IsType<ViewResult>(context.Result);
            Assert.Equal("Error", viewResult.ViewName);
            Assert.False((bool)viewResult.ViewData["ShowException"]);
            Assert.Equal("Exception", (string)viewResult.ViewData["ExceptionType"]);
            Assert.Equal("Some message", (string)viewResult.ViewData["ExceptionMessage"]);
            Assert.NotNull((string)viewResult.ViewData["RequestId"]);
        }

        private static ExceptionContext CreateExceptionContext()
        {
            var actionContext = new ActionContext(new DefaultHttpContext(), new RouteData(), new ActionDescriptor());
            var filters = new List<IFilterMetadata>();
            var context = new ExceptionContext(actionContext, filters)
            {
                Exception = new Exception("Some message"),
            };
            return context;
        }

        private static GeneralExceptionFilter CreateFilterFor(IWebHostEnvironment webHostEnvironment)
        {
            var modelMetadataProvider = new DefaultModelMetadataProvider(new DummyCompositeMetadataDetailsProvider());
            var filter = new GeneralExceptionFilter(webHostEnvironment, modelMetadataProvider);
            return filter;
        }
    }
}
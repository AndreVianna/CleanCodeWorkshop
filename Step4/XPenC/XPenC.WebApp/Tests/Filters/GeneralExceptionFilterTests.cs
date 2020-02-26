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
using XPenC.WebApp.Filters;
using XPenC.WebApp.Models.Shared;
using XPenC.WebApp.Tests.TestDoubles;
using Xunit;

namespace XPenC.WebApp.Tests.Filters
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
            var model = Assert.IsType<ErrorData>(viewResult.Model);
            Assert.NotNull(model.RequestId);
            Assert.True(model.ShowException);
            Assert.NotNull(model.Exception);
        }

        [Fact]
        public void HomeController_OnException_ForDevelopment_WithActivityId_ShouldPass()
        {
            var filter = CreateFilterFor(new FakeDevelopmentEnvironment());
            var context = CreateExceptionContext();
            var activity = new Activity("HomeController_OnException_ForDevelopment_WithActivityId_ShouldPass");
            activity.Start();
            
            filter.OnException(context);

            activity.Stop();
            var viewResult = Assert.IsType<ViewResult>(context.Result);
            Assert.Equal("Error", viewResult.ViewName);
            var model = Assert.IsType<ErrorData>(viewResult.Model);
            Assert.NotNull(model.RequestId);
            Assert.True(model.ShowException);
            Assert.NotNull(model.Exception);
        }

        [Fact]
        public void HomeController_OnException_NotForDevelopment_ShouldPass()
        {
            var filter = CreateFilterFor(new FakeProductionEnvironment());
            var context = CreateExceptionContext();

            filter.OnException(context);

            var viewResult = Assert.IsType<ViewResult>(context.Result);
            Assert.Equal("Error", viewResult.ViewName);
            var model = Assert.IsType<ErrorData>(viewResult.Model);
            Assert.NotNull(model.RequestId);
            Assert.False(model.ShowException);
            Assert.NotNull(model.Exception);
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
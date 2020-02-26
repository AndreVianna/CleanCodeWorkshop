using System.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Hosting;

namespace XPenC.WebApp.Filters
{
    public class GeneralExceptionFilter : IExceptionFilter
    {
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly IModelMetadataProvider _modelMetadataProvider;

        public GeneralExceptionFilter(IWebHostEnvironment hostingEnvironment, IModelMetadataProvider modelMetadataProvider)
        {
            _hostingEnvironment = hostingEnvironment;
            _modelMetadataProvider = modelMetadataProvider;
        }

        public void OnException(ExceptionContext context)
        {
            var requestId = Activity.Current?.Id ?? context.HttpContext.TraceIdentifier;
            context.Result = new ViewResult
            {
                ViewName = "Error",
                ViewData = new ViewDataDictionary(_modelMetadataProvider, context.ModelState)
                {
                    ["RequestId"] = requestId,
                    ["ShowException"] = _hostingEnvironment.IsDevelopment(),
                    ["ExceptionType"] = context.Exception.GetType().Name,
                    ["ExceptionMessage"] = context.Exception.Message,
                }
            };
        }
    }
}

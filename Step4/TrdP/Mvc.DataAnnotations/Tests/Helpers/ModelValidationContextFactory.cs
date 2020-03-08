using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using TrdP.Common.TestDoubles;
using TrdP.Mvc.DataAnnotations.Localization.Tests.TestDoubles;

namespace TrdP.Mvc.DataAnnotations.Localization.Tests.Helpers
{
    internal static class ModelValidationContextFactory
    {
        public static ModelValidationContext Create(object subject)
        {
            var actionContext = CreateActionContext(new FakeServiceCollection());
            var testModel = subject as TestModel;
            var modelMetadataIdentity = testModel != null
                ? ModelMetadataIdentity.ForProperty(typeof(string), nameof(TestModel.TestProperty), typeof(TestModel))
                : ModelMetadataIdentity.ForType(subject?.GetType() ?? typeof(object));
            var modelMetadata = new MockedModelMetadata(modelMetadataIdentity);
            var modelMetadataProvider = new MockedModelMetadataProvider();
            return new ModelValidationContext(actionContext, modelMetadata, modelMetadataProvider, testModel, testModel?.TestProperty ?? subject);
        }

        public static ModelValidationContext CreateWithDisplayNames(object subject)
        {
            var actionContext = CreateActionContext(new FakeServiceCollection());
            var testModel = subject as TestModel;
            var modelMetadataIdentity = testModel != null
                ? ModelMetadataIdentity.ForProperty(typeof(string), nameof(TestModel.TestProperty), typeof(TestModel))
                : ModelMetadataIdentity.ForType(subject?.GetType() ?? typeof(object));
            var modelMetadata = new MockedModelMetadata(modelMetadataIdentity, SetDisplayNames);
            var modelMetadataProvider = new MockedModelMetadataProvider(SetDisplayNames);
            return new ModelValidationContext(actionContext, modelMetadata, modelMetadataProvider, testModel, testModel?.TestProperty ?? subject);
        }

        private static ActionContext CreateActionContext(IServiceCollection services)
        {
            var httpContext = new DefaultHttpContext
            {
                RequestServices = services?.BuildServiceProvider(),
            };
            var routeData = new RouteData();
            var actionDescriptor = new ActionDescriptor();
            var actionContext = new ActionContext(httpContext, routeData, actionDescriptor);
            return actionContext;
        }

        private static void SetDisplayNames(MockedModelMetadata modelMetadata)
        {
            if (modelMetadata.Name == nameof(TestModel.TestProperty))
            {
                modelMetadata.SetDisplayName("Named");
            }

            if (modelMetadata.Name == nameof(TestModel.OtherProperty))
            {
                modelMetadata.SetDisplayName("Other");
            }

            if (modelMetadata.ModelType == typeof(TestModel))
            {
                ((MockedModelMetadata)modelMetadata.Properties[nameof(TestModel.TestProperty)]).SetDisplayName("Named");
                ((MockedModelMetadata)modelMetadata.Properties[nameof(TestModel.OtherProperty)]).SetDisplayName("Other");
            }
        }
    }
}
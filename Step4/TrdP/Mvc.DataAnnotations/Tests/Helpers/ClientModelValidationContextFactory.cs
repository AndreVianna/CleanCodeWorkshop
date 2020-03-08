using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Routing;
using TrdP.Mvc.DataAnnotations.Localization.Tests.TestDoubles;

namespace TrdP.Mvc.DataAnnotations.Localization.Tests.Helpers
{
    internal static class ClientModelValidationContextFactory
    {
        public static ClientModelValidationContext CreateForProperty()
        {
            var actionContext = CreateActionContext();
            var modelMetadataIdentity = ModelMetadataIdentity.ForProperty(typeof(string), nameof(TestModel.TestProperty), typeof(TestModel));
            var modelMetadata = new MockedModelMetadata(modelMetadataIdentity);
            modelMetadata.SetModelBindingMessageProvider(new DefaultModelBindingMessageProvider());
            var modelMetadataProvider = new MockedModelMetadataProvider();
            var attributes = new Dictionary<string, string>();
            return new ClientModelValidationContext(actionContext, modelMetadata, modelMetadataProvider, attributes);
        }

        public static ClientModelValidationContext CreateForPropertyWithDisplayName()
        {
            var actionContext = CreateActionContext();
            var modelMetadataIdentity = ModelMetadataIdentity.ForProperty(typeof(string), nameof(TestModel.TestProperty), typeof(TestModel));
            var modelMetadata = new MockedModelMetadata(modelMetadataIdentity, SetDisplayNames);
            modelMetadata.SetModelBindingMessageProvider(new DefaultModelBindingMessageProvider());
            var modelMetadataProvider = new MockedModelMetadataProvider(SetDisplayNames);
            var attributes = new Dictionary<string, string>();
            return new ClientModelValidationContext(actionContext, modelMetadata, modelMetadataProvider, attributes);
        }

        public static ClientModelValidationContext CreateForType()
        {
            var actionContext = CreateActionContext();
            var modelMetadataIdentity = ModelMetadataIdentity.ForType(typeof(string));
            var modelMetadata = new MockedModelMetadata(modelMetadataIdentity);
            modelMetadata.SetModelBindingMessageProvider(new DefaultModelBindingMessageProvider());
            var modelMetadataProvider = new MockedModelMetadataProvider();
            var attributes = new Dictionary<string, string>();
            return new ClientModelValidationContext(actionContext, modelMetadata, modelMetadataProvider, attributes);
        }

        private static ActionContext CreateActionContext()
        {
            var httpContext = new DefaultHttpContext();
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
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;

namespace TrdP.Mvc.DataAnnotations.Localization.Tests.TestDoubles
{
    internal class MockedModelMetadataProvider : IModelMetadataProvider
    {
        private readonly Action<MockedModelMetadata> _modelMetadataSetupAction;

        public MockedModelMetadataProvider(Action<MockedModelMetadata> modelMetadataSetupAction = null)
        {
            _modelMetadataSetupAction = modelMetadataSetupAction;
        }

        public IEnumerable<ModelMetadata> GetMetadataForProperties(Type modelType) => throw new NotImplementedException();
        public ModelMetadata GetMetadataForType(Type modelType)
        {
            var modelMetadataIdentity = ModelMetadataIdentity.ForType(modelType);
            var modelMetadata = new MockedModelMetadata(modelMetadataIdentity);
            var properties = modelType.GetProperties().Select(i => new MockedModelMetadata(ModelMetadataIdentity.ForProperty(i.GetType(), i.Name, modelType))).ToArray();
            modelMetadata.SetProperties(new ModelPropertyCollection(properties));
            _modelMetadataSetupAction?.Invoke(modelMetadata);
            return modelMetadata;
        }
    }
}
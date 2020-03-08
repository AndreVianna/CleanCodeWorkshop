using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using TrdP.Mvc.DataAnnotations.Localization.Tests.TestDoubles;
using TrdP.Mvc.DataAnnotations.Localization.ValidationProviders;
using Xunit;

namespace TrdP.Mvc.DataAnnotations.Localization.Tests.ValidationProviders
{
    public class ModelValidatorProviderTests
    {
        private readonly ModelValidatorProvider _modelValidatorProvider;

        public ModelValidatorProviderTests()
        {
            var attributeAdapterProvider = new ValidationAttributeAdapterFactory();
            var stringLocalizerFactory = new FakeStringLocalizerFactory();
            _modelValidatorProvider = new ModelValidatorProvider(attributeAdapterProvider, stringLocalizerFactory);
        }

        [Fact]
        public void ModelValidatorProvider_Constructor_WithNullProvider_ShouldThrow()
        {
            Assert.Throws<ArgumentNullException>(() => new ModelValidatorProvider(null, null));
        }

        [Fact]
        public void ModelValidatorProvider_Constructor_WithNullLocalizerFactory_ShouldThrow()
        {
            var attributeAdapterProvider = new ValidationAttributeAdapterFactory();
            Assert.Throws<ArgumentNullException>(() => new ModelValidatorProvider(attributeAdapterProvider, null));
        }

        [Fact]
        public void ModelValidatorProvider_CreateValidators_WithNullContext_ShouldThrow()
        {
            Assert.Throws<ArgumentNullException>(() => _modelValidatorProvider.CreateValidators(null));
        }

        [Fact]
        public void ModelValidatorProvider_HasValidators_WithNoValidator_ShouldPass()
        {
            _modelValidatorProvider.HasValidators(typeof(object), new List<object>());
        }

        [Fact]
        public void ModelValidatorProvider_HasValidators_WithValidatableObject_ShouldPass()
        {
            _modelValidatorProvider.HasValidators(typeof(TestModel), new List<object>());
        }

        [Fact]
        public void ModelValidatorProvider_HasValidators_WithValidationAttribute_ShouldPass()
        {
            _modelValidatorProvider.HasValidators(typeof(object), new List<object>{ new RequiredAttribute() });
        }

        [Fact]
        public void ModelValidatorProvider_CreateValidators_ForType_ShouldPass()
        {
            var modelMetadataIdentity = ModelMetadataIdentity.ForType(typeof(TestModel));
            var modelMetadata = new MockedModelMetadata(modelMetadataIdentity);
            var clientValidationItems = new List<ValidatorItem>();
            var context = new ModelValidatorProviderContext(modelMetadata, clientValidationItems);
            _modelValidatorProvider.CreateValidators(context);
        }

        [Fact]
        public void ModelValidatorProvider_CreateValidators_ForProperty_ShouldPass()
        {
            var modelMetadataIdentity = ModelMetadataIdentity.ForProperty(typeof(string), "TestProperty", typeof(TestModel));
            var modelMetadata = new MockedModelMetadata(modelMetadataIdentity);
            var clientValidationItems = new List<ValidatorItem>();
            var context = new ModelValidatorProviderContext(modelMetadata, clientValidationItems);
            _modelValidatorProvider.CreateValidators(context);
        }

        [Fact]
        public void ModelValidatorProvider_CreateValidators_WithRequiredAttribute_ShouldPass()
        {
            var modelMetadataIdentity = ModelMetadataIdentity.ForProperty(typeof(string), "TestProperty", typeof(TestModel));
            var modelMetadata = new MockedModelMetadata(modelMetadataIdentity);
            var clientValidationItems = new List<ValidatorItem>();
            var context = new ModelValidatorProviderContext(modelMetadata, clientValidationItems);
            context.Results.Add(new ValidatorItem(new MaxLengthAttribute()));
            context.Results.Add(new ValidatorItem(new RequiredAttribute()));
            _modelValidatorProvider.CreateValidators(context);
        }

        [Fact]
        public void ModelValidatorProvider_CreateValidators_WithoutRequiredAttribute_ShouldPass()
        {
            var modelMetadataIdentity = ModelMetadataIdentity.ForProperty(typeof(string), "TestProperty", typeof(TestModel));
            var modelMetadata = new MockedModelMetadata(modelMetadataIdentity);
            modelMetadata.SetIsRequired(true);
            var clientValidationItems = new List<ValidatorItem>();
            var context = new ModelValidatorProviderContext(modelMetadata, clientValidationItems);
            context.Results.Add(new ValidatorItem(new MaxLengthAttribute()));
            context.Results.Add(new ValidatorItem(new MinLengthAttribute(3)));
            _modelValidatorProvider.CreateValidators(context);
        }

        [Fact]
        public void ModelValidatorProvider_CreateValidators_WithUnknownAttribute_ShouldPass()
        {
            var modelMetadataIdentity = ModelMetadataIdentity.ForProperty(typeof(string), "TestProperty", typeof(TestModel));
            var modelMetadata = new MockedModelMetadata(modelMetadataIdentity);
            var clientValidationItems = new List<ValidatorItem>();
            var context = new ModelValidatorProviderContext(modelMetadata, clientValidationItems);
            context.Results.Add(new ValidatorItem("NotAnAttribute"));
            _modelValidatorProvider.CreateValidators(context);
        }

        private class TestModel : IValidatableObject
        {
            public IEnumerable<ValidationResult> Validate(ValidationContext validationContext) => throw new NotImplementedException();
        }
    }
}
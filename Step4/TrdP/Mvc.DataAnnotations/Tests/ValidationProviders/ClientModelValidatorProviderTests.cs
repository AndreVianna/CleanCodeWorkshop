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
    public class ClientModelValidatorProviderTests
    {
        private readonly ClientModelValidatorProvider _validatorProvider;

        public ClientModelValidatorProviderTests()
        {
            var attributeAdapterProvider = new ValidationAttributeAdapterFactory();
            var stringLocalizerFactory = new FakeStringLocalizerFactory();
            _validatorProvider = new ClientModelValidatorProvider(attributeAdapterProvider, stringLocalizerFactory);
        }

        [Fact]
        public void ClientModelValidatorProvider_Constructor_WithNullProvider_ShouldThrow()
        {
            Assert.Throws<ArgumentNullException>(() => new ClientModelValidatorProvider(null, null));
        }

        [Fact]
        public void ClientModelValidatorProvider_Constructor_WithNullLocalizerFactory_ShouldThrow()
        {
            var attributeAdapterProvider = new ValidationAttributeAdapterFactory();
            Assert.Throws<ArgumentNullException>(() => new ClientModelValidatorProvider(attributeAdapterProvider, null));
        }

        [Fact]
        public void ClientModelValidatorProvider_CreateValidators_WithNullContext_ShouldThrow()
        {
            Assert.Throws<ArgumentNullException>(() => _validatorProvider.CreateValidators(null));
        }

        [Fact]
        public void ClientModelValidatorProvider_CreateValidators_ForType_ShouldPass()
        {
            var modelMetadataIdentity = ModelMetadataIdentity.ForType(typeof(TestModel));
            var modelMetadata = new MockedModelMetadata(modelMetadataIdentity);
            var clientValidationItems = new List<ClientValidatorItem>();
            var context = new ClientValidatorProviderContext(modelMetadata, clientValidationItems);
            _validatorProvider.CreateValidators(context);
        }

        [Fact]
        public void ClientModelValidatorProvider_CreateValidators_ForProperty_ShouldPass()
        {
            var modelMetadataIdentity = ModelMetadataIdentity.ForProperty(typeof(string), "TestProperty", typeof(TestModel));
            var modelMetadata = new MockedModelMetadata(modelMetadataIdentity);
            var clientValidationItems = new List<ClientValidatorItem>();
            var context = new ClientValidatorProviderContext(modelMetadata, clientValidationItems);
            _validatorProvider.CreateValidators(context);
        }

        [Fact]
        public void ClientModelValidatorProvider_CreateValidators_WithRequiredAttribute_ShouldPass()
        {
            var modelMetadataIdentity = ModelMetadataIdentity.ForProperty(typeof(string), "TestProperty", typeof(TestModel));
            var modelMetadata = new MockedModelMetadata(modelMetadataIdentity);
            var clientValidationItems = new List<ClientValidatorItem>();
            var context = new ClientValidatorProviderContext(modelMetadata, clientValidationItems);
            context.Results.Add(new ClientValidatorItem(new MaxLengthAttribute()));
            context.Results.Add(new ClientValidatorItem(new RequiredAttribute()));
            _validatorProvider.CreateValidators(context);
        }

        [Fact]
        public void ClientModelValidatorProvider_CreateValidators_WithoutRequiredAttribute_ShouldPass()
        {
            var modelMetadataIdentity = ModelMetadataIdentity.ForProperty(typeof(string), "TestProperty", typeof(TestModel));
            var modelMetadata = new MockedModelMetadata(modelMetadataIdentity);
            modelMetadata.SetIsRequired(true);
            var clientValidationItems = new List<ClientValidatorItem>();
            var context = new ClientValidatorProviderContext(modelMetadata, clientValidationItems);
            context.Results.Add(new ClientValidatorItem(new MaxLengthAttribute()));
            context.Results.Add(new ClientValidatorItem(new MinLengthAttribute(3)));
            _validatorProvider.CreateValidators(context);
        }

        [Fact]
        public void ClientModelValidatorProvider_CreateValidators_WithUnknownAttribute_ShouldPass()
        {
            var modelMetadataIdentity = ModelMetadataIdentity.ForProperty(typeof(string), "TestProperty", typeof(TestModel));
            var modelMetadata = new MockedModelMetadata(modelMetadataIdentity);
            var clientValidationItems = new List<ClientValidatorItem>();
            var context = new ClientValidatorProviderContext(modelMetadata, clientValidationItems);
            context.Results.Add(new ClientValidatorItem("NotAnAttribute"));
            _validatorProvider.CreateValidators(context);
        }

        private class TestModel
        {
        }
    }
}
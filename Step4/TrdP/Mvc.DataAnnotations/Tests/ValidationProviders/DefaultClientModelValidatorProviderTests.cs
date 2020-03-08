using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using TrdP.Mvc.DataAnnotations.Localization.Tests.TestDoubles;
using TrdP.Mvc.DataAnnotations.Localization.ValidationProviders;
using Xunit;

namespace TrdP.Mvc.DataAnnotations.Localization.Tests.ValidationProviders
{
    public class DefaultClientModelValidatorProviderTests
    {
        private readonly DefaultClientModelValidatorProvider _validatorProvider;

        public DefaultClientModelValidatorProviderTests()
        {
            _validatorProvider = new DefaultClientModelValidatorProvider();
        }

        [Fact]
        public void DefaultClientModelValidatorProvider_CreateValidators_WithNullContext_ShouldThrow()
        {
            Assert.Throws<ArgumentNullException>(() => _validatorProvider.CreateValidators(null));
        }

        [Fact]
        public void DefaultClientModelValidatorProvider_CreateValidators_WithoutClientModelValidator_ShouldPass()
        {
            var modelMetadataIdentity = ModelMetadataIdentity.ForType(typeof(string));
            var modelMetadata = new MockedModelMetadata(modelMetadataIdentity);
            var clientValidationItems = new List<ClientValidatorItem>();
            var context = new ClientValidatorProviderContext(modelMetadata, clientValidationItems);
            _validatorProvider.CreateValidators(context);
        }

        [Fact]
        public void DefaultClientModelValidatorProvider_CreateValidators_WithClientModelValidator_ShouldPass()
        {
            var modelMetadataIdentity = ModelMetadataIdentity.ForType(typeof(string));
            var modelMetadata = new MockedModelMetadata(modelMetadataIdentity);
            var clientValidationItems = new List<ClientValidatorItem>();
            var context = new ClientValidatorProviderContext(modelMetadata, clientValidationItems);
            context.Results.Add(new ClientValidatorItem(new DummyClientModelValidator()));
            _validatorProvider.CreateValidators(context);
        }

        [Fact]
        public void DefaultClientModelValidatorProvider_CreateValidators_WithInvalidClientModelValidator_ShouldPass()
        {
            var modelMetadataIdentity = ModelMetadataIdentity.ForType(typeof(string));
            var modelMetadata = new MockedModelMetadata(modelMetadataIdentity);
            var clientValidationItems = new List<ClientValidatorItem>();
            var context = new ClientValidatorProviderContext(modelMetadata, clientValidationItems);
            context.Results.Add(new ClientValidatorItem("NotAValidator"));
            _validatorProvider.CreateValidators(context);
        }
    }
}
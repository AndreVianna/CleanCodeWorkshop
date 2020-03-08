using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using TrdP.Mvc.DataAnnotations.Localization.Tests.TestDoubles;
using TrdP.Mvc.DataAnnotations.Localization.ValidationProviders;
using Xunit;

namespace TrdP.Mvc.DataAnnotations.Localization.Tests.ValidationProviders
{
    public class DefaultModelValidatorProviderTests
    {
        private readonly DefaultModelValidatorProvider _validatorProvider;

        public DefaultModelValidatorProviderTests()
        {
            _validatorProvider = new DefaultModelValidatorProvider();
        }

        [Fact]
        public void DefaultModelValidatorProvider_CreateValidators_WithNullContext_ShouldThrow()
        {
            Assert.Throws<ArgumentNullException>(() => _validatorProvider.CreateValidators(null));
        }

        [Fact]
        public void DefaultModelValidatorProvider_CreateValidators_WithoutAttributes_ShouldPass()
        {
            var modelMetadataIdentity = ModelMetadataIdentity.ForType(typeof(string));
            var modelMetadata = new MockedModelMetadata(modelMetadataIdentity);
            var clientValidationItems = new List<ValidatorItem>();
            var context = new ModelValidatorProviderContext(modelMetadata, clientValidationItems);
            _validatorProvider.CreateValidators(context);
        }

        [Fact]
        public void DefaultModelValidatorProvider_CreateValidators_WithAttributes_ShouldPass()
        {
            var modelMetadataIdentity = ModelMetadataIdentity.ForType(typeof(string));
            var modelMetadata = new MockedModelMetadata(modelMetadataIdentity);
            var clientValidationItems = new List<ValidatorItem>();
            var context = new ModelValidatorProviderContext(modelMetadata, clientValidationItems);
            context.Results.Add(new ValidatorItem(new DummyModelValidator()));
            _validatorProvider.CreateValidators(context);
        }

        [Fact]
        public void DefaultModelValidatorProvider_CreateValidators_WithUnknownAttribute_ShouldPass()
        {
            var modelMetadataIdentity = ModelMetadataIdentity.ForType(typeof(string));
            var modelMetadata = new MockedModelMetadata(modelMetadataIdentity);
            var clientValidationItems = new List<ValidatorItem>();
            var context = new ModelValidatorProviderContext(modelMetadata, clientValidationItems);
            context.Results.Add(new ValidatorItem("NotAnAttribute"));
            _validatorProvider.CreateValidators(context);
        }
    }
}
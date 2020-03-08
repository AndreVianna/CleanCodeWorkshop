using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using TrdP.Mvc.DataAnnotations.Localization.ClientModelValidators;
using TrdP.Mvc.DataAnnotations.Localization.ModelValidators;
using TrdP.Mvc.DataAnnotations.Localization.Tests.TestDoubles;
using Xunit;
using NumericClientModelValidatorProvider = TrdP.Mvc.DataAnnotations.Localization.ValidationProviders.NumericClientModelValidatorProvider;

namespace TrdP.Mvc.DataAnnotations.Localization.Tests.ValidationProviders
{
    public class NumericClientModelValidatorProviderTests
    {
        private readonly NumericClientModelValidatorProvider _validatorProvider;

        public NumericClientModelValidatorProviderTests()
        {
            _validatorProvider = new NumericClientModelValidatorProvider();
        }

        [Fact]
        public void ClientModelValidatorProvider_CreateValidators_WithNullContext_ShouldThrow()
        {
            Assert.Throws<ArgumentNullException>(() => _validatorProvider.CreateValidators(null));
        }

        [Fact]
        public void ClientModelValidatorProvider_CreateValidators_ForNonNumeric_ShouldPass()
        {
            var modelMetadataIdentity = ModelMetadataIdentity.ForType(typeof(string));
            var modelMetadata = new MockedModelMetadata(modelMetadataIdentity);
            var clientValidationItems = new List<ClientValidatorItem>();
            var context = new ClientValidatorProviderContext(modelMetadata, clientValidationItems);
            _validatorProvider.CreateValidators(context);
        }

        [Fact]
        public void ClientModelValidatorProvider_CreateValidators_ForFloat_ShouldPass()
        {
            var modelMetadataIdentity = ModelMetadataIdentity.ForType(typeof(float));
            var modelMetadata = new MockedModelMetadata(modelMetadataIdentity);
            var clientValidationItems = new List<ClientValidatorItem>();
            var context = new ClientValidatorProviderContext(modelMetadata, clientValidationItems);
            _validatorProvider.CreateValidators(context);
        }

        [Fact]
        public void ClientModelValidatorProvider_CreateValidators_ForDouble_ShouldPass()
        {
            var modelMetadataIdentity = ModelMetadataIdentity.ForType(typeof(double));
            var modelMetadata = new MockedModelMetadata(modelMetadataIdentity);
            var clientValidationItems = new List<ClientValidatorItem>();
            var context = new ClientValidatorProviderContext(modelMetadata, clientValidationItems);
            _validatorProvider.CreateValidators(context);
        }

        [Fact]
        public void ClientModelValidatorProvider_CreateValidators_ForDecimal_ShouldPass()
        {
            var modelMetadataIdentity = ModelMetadataIdentity.ForType(typeof(decimal));
            var modelMetadata = new MockedModelMetadata(modelMetadataIdentity);
            var clientValidationItems = new List<ClientValidatorItem>();
            var context = new ClientValidatorProviderContext(modelMetadata, clientValidationItems);
            _validatorProvider.CreateValidators(context);
        }

        [Fact]
        public void ClientModelValidatorProvider_CreateValidators_WithNumericValidator_ShouldPass()
        {
            var modelMetadataIdentity = ModelMetadataIdentity.ForType(typeof(decimal));
            var modelMetadata = new MockedModelMetadata(modelMetadataIdentity);
            modelMetadata.SetIsRequired(true);
            var clientValidationItems = new List<ClientValidatorItem>();
            var context = new ClientValidatorProviderContext(modelMetadata, clientValidationItems);
            context.Results.Add(new ClientValidatorItem { Validator = new NumericClientModelValidator() });
            _validatorProvider.CreateValidators(context);
        }

        [Fact]
        public void ClientModelValidatorProvider_CreateValidators_WithUnknownValidator_ShouldPass()
        {
            var modelMetadataIdentity = ModelMetadataIdentity.ForType(typeof(decimal));
            var modelMetadata = new MockedModelMetadata(modelMetadataIdentity);
            var clientValidationItems = new List<ClientValidatorItem>();
            var context = new ClientValidatorProviderContext(modelMetadata, clientValidationItems);
            context.Results.Add(new ClientValidatorItem("NotAnAttribute"));
            _validatorProvider.CreateValidators(context);
        }
    }
}
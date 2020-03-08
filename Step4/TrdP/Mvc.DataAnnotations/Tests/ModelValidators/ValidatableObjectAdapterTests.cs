using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using TrdP.Mvc.DataAnnotations.Localization.ModelValidators;
using TrdP.Mvc.DataAnnotations.Localization.Tests.Helpers;
using TrdP.Mvc.DataAnnotations.Localization.Tests.TestDoubles;
using Xunit;

namespace TrdP.Mvc.DataAnnotations.Localization.Tests.ModelValidators
{
    public class ValidatableObjectAdapterTests
    {
        private readonly ValidatableObjectAdapter _validatableObjectAdapter;

        public ValidatableObjectAdapterTests()
        {
            _validatableObjectAdapter = new ValidatableObjectAdapter();
        }

        [Fact]
        public void ValidatableObjectAdapter_Validate_WithNullContext_ShouldThrow()
        {
            Assert.Throws<ArgumentNullException>(() => _validatableObjectAdapter.Validate(null));
        }

        [Fact]
        public void ValidatableObjectAdapter_Validate_WithNullModel_ShouldPass()
        {
            var context = ModelValidationContextFactory.Create(null);
            var result = _validatableObjectAdapter.Validate(context);
            Assert.Empty(result);
        }

        [Fact]
        public void ValidatableObjectAdapter_Validate_WithNonValidatableObject_ShouldThrow()
        {
            var context = ModelValidationContextFactory.Create("string");
            Assert.Throws<InvalidOperationException>(() => _validatableObjectAdapter.Validate(context));
        }

        [Fact]
        public void ValidatableObjectAdapter_Validate_ForNullHttpContext_ShouldPass()
        {
            var actionContext = new ActionContext();
            var modelMetadataIdentity = ModelMetadataIdentity.ForProperty(typeof(string), nameof(TestModel.TestProperty), typeof(TestModel));
            var modelMetadata = new MockedModelMetadata(modelMetadataIdentity);
            var modelMetadataProvider = new MockedModelMetadataProvider();
            var context = new ModelValidationContext(actionContext, modelMetadata, modelMetadataProvider, null, new TestValidatableObject(null));
            var result = _validatableObjectAdapter.Validate(context);
            Assert.Empty(result);
        }

        [Fact]
        public void ValidatableObjectAdapter_Validate_WithoutErrorsAsNull_ShouldPass()
        {
            var context = ModelValidationContextFactory.Create(new TestValidatableObject(null));
            var result = _validatableObjectAdapter.Validate(context);
            Assert.Empty(result);
        }

        [Fact]
        public void ValidatableObjectAdapter_Validate_WithoutErrors_ShouldPass()
        {
            var errors = Enumerable.Empty<ValidationResult>();
            var context = ModelValidationContextFactory.Create(new TestValidatableObject(errors));
            var result = _validatableObjectAdapter.Validate(context);
            Assert.Empty(result);
        }

        [Fact]
        public void ValidatableObjectAdapter_Validate_WithErrors_ShouldPass()
        {
            var errors = new List<ValidationResult>
            {
                null,
                ValidationResult.Success,
                new ValidationResult("ErrorMessage1"),
                new ValidationResult("ErrorMessage2", new [] {"Member1", "Member2", null}),
            };
            var context = ModelValidationContextFactory.Create(new TestValidatableObject(errors));
            var result = _validatableObjectAdapter.Validate(context);
            Assert.NotEmpty(result);
        }
    }
}
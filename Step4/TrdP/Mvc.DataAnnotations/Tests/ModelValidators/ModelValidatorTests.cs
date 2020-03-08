using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using TrdP.Localization.Abstractions;
using TrdP.Mvc.DataAnnotations.Localization.AttributeAdapters;
using TrdP.Mvc.DataAnnotations.Localization.ModelValidators;
using TrdP.Mvc.DataAnnotations.Localization.Tests.Helpers;
using TrdP.Mvc.DataAnnotations.Localization.Tests.TestDoubles;
using TrdP.Mvc.DataAnnotations.Localization.ValidationProviders;
using Xunit;

namespace TrdP.Mvc.DataAnnotations.Localization.Tests.ModelValidators
{
    public class ModelValidatorTests
    {
        private readonly ModelValidator _validatorProvider;

        public ModelValidatorTests()
        {
            var attributeAdapterProvider = new ValidationAttributeAdapterFactory();
            var attribute = new MaxLengthAttribute(10);
            var stringLocalizer = new FakeStringLocalizer();
            _validatorProvider = new ModelValidator(attributeAdapterProvider, attribute, stringLocalizer);
        }

        [Fact]
        public void ModelValidator_Constructor_WithNullFactory_ShouldThrow()
        {
            Assert.Throws<ArgumentNullException>(() => new ModelValidator(null, null));
        }

        [Fact]
        public void ModelValidator_Constructor_WithNullAttribute_ShouldThrow()
        {
            var attributeAdapterProvider = new ValidationAttributeAdapterFactory();
            Assert.Throws<ArgumentNullException>(() => new ModelValidator(attributeAdapterProvider, null));
        }

        [Fact]
        public void ModelValidator_Validate_WithNullContext_ShouldThrow()
        {
            Assert.Throws<ArgumentNullException>(() => _validatorProvider.Validate(null));
        }

        [Fact]
        public void ModelValidator_Validate_WithoutErrors_ForContainer_ShouldPass()
        {
            var context = ModelValidationContextFactory.Create(new TestModel { TestProperty = "String" });
            var result = _validatorProvider.Validate(context);
            Assert.Empty(result);
        }

        [Fact]
        public void ModelValidator_Validate_WithoutErrors_ForModel_ShouldPass()
        {
            var context = ModelValidationContextFactory.Create("String");
            var result = _validatorProvider.Validate(context);
            Assert.Empty(result);
        }

        [Fact]
        public void ModelValidator_Validate_WithoutErrors_ForNoObject_ShouldPass()
        {
            var context = ModelValidationContextFactory.Create(null);
            var result = _validatorProvider.Validate(context);
            Assert.Empty(result);
        }

        [Fact]
        public void ModelValidator_Validate_ForNullHttpContext_ShouldPass()
        {
            var actionContext = new ActionContext();
            var modelMetadataIdentity = ModelMetadataIdentity.ForProperty(typeof(string), nameof(TestModel.TestProperty), typeof(TestModel));
            var modelMetadata = new MockedModelMetadata(modelMetadataIdentity);
            var modelMetadataProvider = new MockedModelMetadataProvider();
            var context = new ModelValidationContext(actionContext, modelMetadata, modelMetadataProvider, null, null);
            var result = _validatorProvider.Validate(context);
            Assert.Empty(result);
        }

        [Fact]
        public void ModelValidator_Validate_WithErrors_ShouldPass()
        {
            var context = ModelValidationContextFactory.Create(new TestModel { TestProperty = "StringTooLong" });
            var result = _validatorProvider.Validate(context);
            Assert.NotEmpty(result);
        }

        [Fact]
        public void ModelValidator_Validate_WithErrors_ForType_ShouldPass()
        {
            var context = ModelValidationContextFactory.Create("StringTooLong");
            var result = _validatorProvider.Validate(context);
            Assert.NotEmpty(result);
        }

        [Fact]
        public void ModelValidator_Validate_WithErrorWithMultipleMemberNames_ShouldPass()
        {
            var attributeAdapterProvider = new TestValidationAttributeAdapterFactory();
            var attribute = new TestValidationAttribute();
            var stringLocalizer = new FakeStringLocalizer();
            var validatorProvider = new ModelValidator(attributeAdapterProvider, attribute, stringLocalizer);
            var context = ModelValidationContextFactory.Create(new TestModel { TestProperty = "Some Value" });
            var result = validatorProvider.Validate(context);
            Assert.NotEmpty(result);
        }

        private sealed class TestValidationAttributeAdapterFactory : IValidationAttributeAdapterFactory
        {
            public IValidationAttributeAdapter Create(ValidationAttribute attribute, IStringLocalizer stringLocalizer) 
                => new TestValidationAttributeAdapter((TestValidationAttribute)attribute, stringLocalizer);
        }

        private sealed class TestValidationAttributeAdapter : ValidationAttributeAdapter<TestValidationAttribute>
        {
            public TestValidationAttributeAdapter(TestValidationAttribute attribute, IStringLocalizer stringLocalizer)
                : base(attribute, stringLocalizer)
            {
            }

            protected override void AddAdapterValidation(ClientModelValidationContext context)
            {
            }

            protected override string GetAdapterErrorMessage(ModelValidationContextBase context)
            {
                return Attribute.ErrorMessage;
            }
        }

        private sealed class TestValidationAttribute : ValidationAttribute
        {
            protected override ValidationResult IsValid(object value, ValidationContext validationContext)
            {
                return new ValidationResult("Error Message.", new List<string> { "TestProperty", "OtherProperty" });
            }
        }
    }
}
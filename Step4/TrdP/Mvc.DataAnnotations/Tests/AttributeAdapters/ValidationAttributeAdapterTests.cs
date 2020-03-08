using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using TrdP.Localization.Abstractions;
using TrdP.Mvc.DataAnnotations.Localization.AttributeAdapters;
using TrdP.Mvc.DataAnnotations.Localization.Tests.Helpers;
using TrdP.Mvc.DataAnnotations.Localization.Tests.TestDoubles;
using Xunit;

namespace TrdP.Mvc.DataAnnotations.Localization.Tests.AttributeAdapters
{
    public class ValidationAttributeAdapterTests
    {
        [Fact]
        public void ValidationAttributeAdapter_Constructor_WithNullAttribute_ShouldThrow()
        {
            Assert.Throws<ArgumentNullException>(() => new TestAttributeAdapter(null, null));
        }

        [Fact]
        public void ValidationAttributeAdapter_Constructor_WithNullLocalizer_ShouldThrow()
        {
            Assert.Throws<ArgumentNullException>(() => new TestAttributeAdapter(new TestAttribute(), null));
        }

        [Fact]
        public void ValidationAttributeAdapter_AddValidation_WithNullContext_ShouldThrow()
        {
            var adapter = CreateAdapter();
            Assert.Throws<ArgumentNullException>(() => adapter.AddValidation(null));
        }


        [Fact]
        public void ValidationAttributeAdapter_AddValidation_ShouldPass()
        {
            var context = ClientModelValidationContextFactory.CreateForProperty();
            var adapter = CreateAdapter();
            adapter.AddValidation(context);
        }

        [Fact]
        public void ValidationAttributeAdapter_AddValidationCalledTwice_ShouldPass()
        {
            var context = ClientModelValidationContextFactory.CreateForProperty();
            var adapter = CreateAdapter();
            adapter.AddValidation(context);
            adapter.AddValidation(context);
        }

        [Fact]
        public void CompareAttributeAdapter_GetErrorMessage_WithNullContext_ShouldThrow()
        {
            var adapter = CreateAdapter();
            Assert.Throws<ArgumentNullException>(() => adapter.GetErrorMessage(null));
        }

        [Fact]
        public void ValidationAttributeAdapter_GetErrorMessage_ShouldPass()
        {
            var context = ClientModelValidationContextFactory.CreateForProperty();
            var adapter = CreateAdapter();
            var result = adapter.GetErrorMessage(context);
            Assert.Equal("The field TestProperty is invalid.", result);
        }

        [Fact]
        public void ValidationAttributeAdapter_GetErrorMessage_WithErrorMessage_ShouldPass()
        {
            var context = ClientModelValidationContextFactory.CreateForProperty();
            var adapter = CreateAdapter("The field '{0}' cannot be null or empty.");
            var result = adapter.GetErrorMessage(context);
            Assert.Equal("The field 'TestProperty' cannot be null or empty.", result);
        }

        [Fact]
        public void ValidationAttributeAdapter_GetErrorMessage_WithNullMessage_ShouldPass()
        {
            var context = ClientModelValidationContextFactory.CreateForProperty();
            var adapter = CreateAdapter(null);
            var result = adapter.GetErrorMessage(context);
            Assert.Null(result);
        }

        private static TestAttributeAdapter CreateAdapter()
        {
            var attribute = new TestAttribute();
            var stringLocalizer = new FakeStringLocalizer();
            return new TestAttributeAdapter(attribute, stringLocalizer);
        }

        private static TestAttributeAdapter CreateAdapter(string errorMessage)
        {
            var attribute = new TestAttribute(errorMessage);
            var stringLocalizer = new FakeStringLocalizer();
            return new TestAttributeAdapter(attribute, stringLocalizer);
        }

        private sealed class TestAttribute : ValidationAttribute
        {
            public TestAttribute()
            {
            }

            public TestAttribute(string errorMessage) : base(errorMessage)
            {
            }
        }

        private class TestAttributeAdapter : ValidationAttributeAdapter<TestAttribute>
        {
            public TestAttributeAdapter(TestAttribute attribute, IStringLocalizer stringLocalizer) : base(attribute, stringLocalizer)
            {
            }

            protected override void AddAdapterValidation(ClientModelValidationContext context)
            {
                MergeAttribute(context.Attributes, "data-val", "true");
            }

            protected override string GetAdapterErrorMessage(ModelValidationContextBase context)
            {
                return GetLocalizedErrorMessage("TestProperty");
            }
        }
    }
}
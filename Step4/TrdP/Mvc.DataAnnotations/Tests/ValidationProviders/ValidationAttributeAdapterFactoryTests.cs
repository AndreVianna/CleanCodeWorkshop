using System;
using System.ComponentModel.DataAnnotations;
using TrdP.Mvc.DataAnnotations.Localization.AttributeAdapters;
using TrdP.Mvc.DataAnnotations.Localization.Tests.TestDoubles;
using TrdP.Mvc.DataAnnotations.Localization.ValidationProviders;
using Xunit;

namespace TrdP.Mvc.DataAnnotations.Localization.Tests.ValidationProviders
{
    public class ValidationAttributeAdapterFactoryTests
    {
        [Fact]
        public void ValidationAttributeAdapterFactory_GetAttributeAdapter_ShouldThrow()
        {
            var provider = new ValidationAttributeAdapterFactory();

            Assert.Throws<ArgumentNullException>(() => provider.Create(null, new DummyStringLocalizer()));
        }

        [Fact]
        public void ValidationAttributeAdapterFactory_GetAttributeAdapter_ForRegularExpressionAttribute_ShouldPass()
        {
            var provider = new ValidationAttributeAdapterFactory();

            Assert.IsAssignableFrom<IValidationAttributeAdapter>(provider.Create(new RegularExpressionAttribute("somePattern"), new DummyStringLocalizer()));
        }

        [Fact]
        public void ValidationAttributeAdapterFactory_GetAttributeAdapter_ForMaxLengthAttribute_ShouldPass()
        {
            var provider = new ValidationAttributeAdapterFactory();

            Assert.IsAssignableFrom<IValidationAttributeAdapter>(provider.Create(new MaxLengthAttribute(100), new DummyStringLocalizer()));
        }

        [Fact]
        public void ValidationAttributeAdapterFactory_GetAttributeAdapter_ForMinLengthAttribute_ShouldPass()
        {
            var provider = new ValidationAttributeAdapterFactory();

            Assert.IsAssignableFrom<IValidationAttributeAdapter>(provider.Create(new MinLengthAttribute(3), new DummyStringLocalizer()));
        }

        [Fact]
        public void ValidationAttributeAdapterFactory_GetAttributeAdapter_ForRequiredAttribute_ShouldPass()
        {
            var provider = new ValidationAttributeAdapterFactory();

            Assert.IsAssignableFrom<IValidationAttributeAdapter>(provider.Create(new RequiredAttribute(), new DummyStringLocalizer()));
        }

        [Fact]
        public void ValidationAttributeAdapterFactory_GetAttributeAdapter_ForCompareAttribute_ShouldPass()
        {
            var provider = new ValidationAttributeAdapterFactory();

            Assert.IsAssignableFrom<IValidationAttributeAdapter>(provider.Create(new CompareAttribute("someOtherProperty"), new DummyStringLocalizer()));
        }

        [Fact]
        public void ValidationAttributeAdapterFactory_GetAttributeAdapter_ForStringLengthAttribute_ShouldPass()
        {
            var provider = new ValidationAttributeAdapterFactory();

            Assert.IsAssignableFrom<IValidationAttributeAdapter>(provider.Create(new StringLengthAttribute(100), new DummyStringLocalizer()));
        }

        [Fact]
        public void ValidationAttributeAdapterFactory_GetAttributeAdapter_ForCreditCardAttribute_ShouldPass()
        {
            var provider = new ValidationAttributeAdapterFactory();

            Assert.IsAssignableFrom<IValidationAttributeAdapter>(provider.Create(new CreditCardAttribute(), new DummyStringLocalizer()));
        }

        [Fact]
        public void ValidationAttributeAdapterFactory_GetAttributeAdapter_ForRangeAttribute_ShouldPass()
        {
            var provider = new ValidationAttributeAdapterFactory();

            Assert.IsAssignableFrom<IValidationAttributeAdapter>(provider.Create(new RangeAttribute(8, 80), new DummyStringLocalizer()));
        }

        [Fact]
        public void ValidationAttributeAdapterFactory_GetAttributeAdapter_ForEmailAddressAttribute_ShouldPass()
        {
            var provider = new ValidationAttributeAdapterFactory();

            Assert.IsAssignableFrom<IValidationAttributeAdapter>(provider.Create(new EmailAddressAttribute(), new DummyStringLocalizer()));
        }

        [Fact]
        public void ValidationAttributeAdapterFactory_GetAttributeAdapter_ForPhoneAttribute_ShouldPass()
        {
            var provider = new ValidationAttributeAdapterFactory();

            Assert.IsAssignableFrom<IValidationAttributeAdapter>(provider.Create(new PhoneAttribute(), new DummyStringLocalizer()));
        }

        [Fact]
        public void ValidationAttributeAdapterFactory_GetAttributeAdapter_ForUrlAttribute_ShouldPass()
        {
            var provider = new ValidationAttributeAdapterFactory();

            Assert.IsAssignableFrom<IValidationAttributeAdapter>(provider.Create(new UrlAttribute(), new DummyStringLocalizer()));
        }

        [Fact]
        public void ValidationAttributeAdapterFactory_GetAttributeAdapter_ForFileExtensionsAttribute_ShouldPass()
        {
            var provider = new ValidationAttributeAdapterFactory();

            Assert.IsAssignableFrom<IValidationAttributeAdapter>(provider.Create(new FileExtensionsAttribute(), new DummyStringLocalizer()));
        }

        private sealed class UnknownAttribute : ValidationAttribute
        {
        }

        [Fact]
        public void ValidationAttributeAdapterFactory_GetAttributeAdapter_ForUnknownAttribute_ShouldPass()
        {
            var provider = new ValidationAttributeAdapterFactory();

            Assert.Null(provider.Create(new UnknownAttribute(), new DummyStringLocalizer()));
        }
    }
}
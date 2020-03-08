using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using TrdP.Common.TestDoubles;
using TrdP.Localization.Abstractions;
using TrdP.Mvc.DataAnnotations.Localization.Tests.TestDoubles;
using TrdP.Mvc.DataAnnotations.Localization.ValidationProviders;
using Xunit;

namespace TrdP.Mvc.DataAnnotations.Localization.Tests
{
    public class DataAnnotationsLocalizationServicesTests
    {
        private static readonly IServiceCollection _fakeServiceCollection = new FakeServiceCollection();

        [Fact]
        public void DataAnnotationsLocalizationServices_SetDataAnnotationsLocalizationServices_ShouldPass()
        {
            _fakeServiceCollection.TryAddSingleton<IStringLocalizerFactory, FakeStringLocalizerFactory>();
            //_fakeServiceCollection.TryAddTransient(typeof(IStringLocalizer<>), typeof(FakeStringLocalizer<>));

            DataAnnotationsLocalizationServices.SetDataAnnotationsLocalizationServices(_fakeServiceCollection);

            var provider = _fakeServiceCollection.BuildServiceProvider();
            Assert.IsType<ValidationAttributeAdapterFactory>(provider.GetRequiredService<IValidationAttributeAdapterFactory>());

            var configureMvcOptions = provider.GetRequiredService<IConfigureOptions<MvcOptions>>();
            var mvcOptions = new MvcOptions();
            configureMvcOptions.Configure(mvcOptions);
            Assert.NotEmpty(mvcOptions.ModelMetadataDetailsProviders);
            Assert.NotEmpty(mvcOptions.ModelValidatorProviders);
            Assert.Equal("A value is required.", mvcOptions.ModelBindingMessageProvider.MissingKeyOrValueAccessor());
            Assert.Equal("A value for the '123' parameter or property was not provided.", mvcOptions.ModelBindingMessageProvider.MissingBindRequiredValueAccessor("123"));
            Assert.Equal("A non-empty request body is required.", mvcOptions.ModelBindingMessageProvider.MissingRequestBodyRequiredValueAccessor());
            Assert.Equal("The value cannot be null.", mvcOptions.ModelBindingMessageProvider.ValueMustNotBeNullAccessor("123"));
            Assert.Equal("The value '123' is not valid for abc.", mvcOptions.ModelBindingMessageProvider.AttemptedValueIsInvalidAccessor("123", "abc"));
            Assert.Equal("The value '123' is not valid.", mvcOptions.ModelBindingMessageProvider.NonPropertyAttemptedValueIsInvalidAccessor("123"));
            Assert.Equal("The supplied value is invalid for abc.", mvcOptions.ModelBindingMessageProvider.UnknownValueIsInvalidAccessor("abc"));
            Assert.Equal("The supplied value is invalid.", mvcOptions.ModelBindingMessageProvider.NonPropertyUnknownValueIsInvalidAccessor());
            Assert.Equal("The value '123' is invalid.", mvcOptions.ModelBindingMessageProvider.ValueIsInvalidAccessor("123"));
            Assert.Equal("The field abc must be a number.", mvcOptions.ModelBindingMessageProvider.ValueMustBeANumberAccessor("abc"));
            Assert.Equal("The field must be a number.", mvcOptions.ModelBindingMessageProvider.NonPropertyValueMustBeANumberAccessor());

            var configureMvcViewOptions = provider.GetRequiredService<IConfigureOptions<MvcViewOptions>>();
            var mvcViewOptions = new MvcViewOptions();
            configureMvcViewOptions.Configure(mvcViewOptions);
            Assert.NotEmpty(mvcViewOptions.ClientModelValidatorProviders);
        }
    }
}

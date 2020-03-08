using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using TrdP.Localization.Abstractions;
using TrdP.Mvc.DataAnnotations.Localization.ValidationProviders;

namespace TrdP.Mvc.DataAnnotations.Localization
{
    public class MvcOptionsSetup : IConfigureOptions<MvcOptions>
    {
        private readonly IStringLocalizerFactory _stringLocalizerFactory;
        private readonly IValidationAttributeAdapterFactory _validationAttributeAdapterFactory;

        public MvcOptionsSetup(
            IValidationAttributeAdapterFactory validationAttributeAdapterFactory,
            IStringLocalizerFactory stringLocalizerFactory)
        {
            _validationAttributeAdapterFactory = validationAttributeAdapterFactory;
            _stringLocalizerFactory = stringLocalizerFactory;
        }

        public void Configure(MvcOptions options)
        {
            options.ModelMetadataDetailsProviders.Add(new MetadataDetailsProvider(_stringLocalizerFactory));

            options.ModelValidatorProviders.Clear();
            options.ModelValidatorProviders.Add(new DefaultModelValidatorProvider());
            options.ModelValidatorProviders.Add(new ModelValidatorProvider(_validationAttributeAdapterFactory, _stringLocalizerFactory));

            LocalizeModelBindingMessages(options);
        }

        private void LocalizeModelBindingMessages(MvcOptions options)
        {
            var localizer = _stringLocalizerFactory.CreateForSharedResources();
            options.ModelBindingMessageProvider.SetMissingKeyOrValueAccessor(() => localizer["A value is required."]);
            options.ModelBindingMessageProvider.SetMissingBindRequiredValueAccessor(value => localizer["A value for the '{0}' parameter or property was not provided.", value]);
            options.ModelBindingMessageProvider.SetMissingRequestBodyRequiredValueAccessor(() => localizer["A non-empty request body is required."]);
            options.ModelBindingMessageProvider.SetValueMustNotBeNullAccessor(value => localizer["The value cannot be null.", value]);
            options.ModelBindingMessageProvider.SetAttemptedValueIsInvalidAccessor((value, field) => localizer["The value '{0}' is not valid for {1}.", value, field]);
            options.ModelBindingMessageProvider.SetNonPropertyAttemptedValueIsInvalidAccessor(value => localizer["The value '{0}' is not valid.", value]);
            options.ModelBindingMessageProvider.SetUnknownValueIsInvalidAccessor(field => localizer["The supplied value is invalid for {0}.", field]);
            options.ModelBindingMessageProvider.SetNonPropertyUnknownValueIsInvalidAccessor(() => localizer["The supplied value is invalid."]);
            options.ModelBindingMessageProvider.SetValueIsInvalidAccessor(value => localizer["The value '{0}' is invalid.", value]);
            options.ModelBindingMessageProvider.SetValueMustBeANumberAccessor(field => localizer["The field {0} must be a number.", field]);
            options.ModelBindingMessageProvider.SetNonPropertyValueMustBeANumberAccessor(() => localizer["The field must be a number."]);
            
        }
}
}
using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using TrdP.Localization.Abstractions;
using TrdP.Mvc.DataAnnotations.Localization.ValidationProviders;

namespace TrdP.Mvc.DataAnnotations.Localization
{
    internal class DataAnnotationsMvcOptionsSetup : IConfigureOptions<MvcOptions>
    {
        private readonly IStringLocalizerFactory _stringLocalizerFactory;
        private readonly IValidationAttributeAdapterProvider _validationAttributeAdapterProvider;

        public DataAnnotationsMvcOptionsSetup(
            IValidationAttributeAdapterProvider validationAttributeAdapterProvider)
        {
            _validationAttributeAdapterProvider = validationAttributeAdapterProvider ?? throw new ArgumentNullException(nameof(validationAttributeAdapterProvider));
        }

        public DataAnnotationsMvcOptionsSetup(
            IValidationAttributeAdapterProvider validationAttributeAdapterProvider,
            IStringLocalizerFactory stringLocalizerFactory)
            : this(validationAttributeAdapterProvider)
        {
            _stringLocalizerFactory = stringLocalizerFactory;
        }

        public void Configure(MvcOptions options)
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            var localizer = _stringLocalizerFactory.Create("SharedResources");
            options.ModelBindingMessageProvider.SetValueIsInvalidAccessor(x => localizer["The value '{0}' is invalid.", x]);
            options.ModelBindingMessageProvider.SetValueMustBeANumberAccessor(x => localizer["The field {0} must be a number.",x]);
            options.ModelBindingMessageProvider.SetMissingBindRequiredValueAccessor(x => localizer["A value for the '{0}' property was not provided.", x]);
            options.ModelBindingMessageProvider.SetAttemptedValueIsInvalidAccessor((x, y) => localizer["The value '{0}' is not valid for {1}.", x, y]);
            options.ModelBindingMessageProvider.SetMissingKeyOrValueAccessor(() => localizer["A value is required."]);
            options.ModelBindingMessageProvider.SetUnknownValueIsInvalidAccessor(x => localizer["The supplied value is invalid for {0}.", x]);
            options.ModelBindingMessageProvider.SetValueMustNotBeNullAccessor(x => localizer["The value cannot not be null."]);

            options.ModelValidatorProviders.Clear();
            options.ModelValidatorProviders.Add(new DefaultModelValidatorProvider());
            options.ModelValidatorProviders.Add(new DataAnnotationsModelValidatorProvider(_validationAttributeAdapterProvider, _stringLocalizerFactory));
        }
    }
}
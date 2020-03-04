using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using TrdP.Localization.Abstractions;
using TrdP.Mvc.DataAnnotations.Localization.ValidationProviders;

namespace TrdP.Mvc.DataAnnotations.Localization
{
    internal class MvcViewOptionsSetup : IConfigureOptions<MvcViewOptions>
    {
        private readonly IStringLocalizerFactory _stringLocalizerFactory;
        private readonly IValidationAttributeAdapterProvider _validationAttributeAdapterProvider;

        public MvcViewOptionsSetup(
            IValidationAttributeAdapterProvider validationAttributeAdapterProvider)
        {
            _validationAttributeAdapterProvider = validationAttributeAdapterProvider ?? throw new ArgumentNullException(nameof(validationAttributeAdapterProvider));
        }

        public MvcViewOptionsSetup(
            IValidationAttributeAdapterProvider validationAttributeAdapterProvider,
            IStringLocalizerFactory stringLocalizerFactory)
            : this(validationAttributeAdapterProvider)
        {
            _stringLocalizerFactory = stringLocalizerFactory;
        }

        public void Configure(MvcViewOptions options)
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            options.ClientModelValidatorProviders.Clear();
            options.ClientModelValidatorProviders.Add(new DefaultClientModelValidatorProvider());
            options.ClientModelValidatorProviders.Add(new DataAnnotationsClientModelValidatorProvider(_validationAttributeAdapterProvider, _stringLocalizerFactory));
            options.ClientModelValidatorProviders.Add(new NumericClientModelValidatorProvider());
        }
    }
}
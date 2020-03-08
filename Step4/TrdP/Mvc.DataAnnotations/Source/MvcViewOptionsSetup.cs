using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using TrdP.Localization.Abstractions;
using TrdP.Mvc.DataAnnotations.Localization.ValidationProviders;

namespace TrdP.Mvc.DataAnnotations.Localization
{
    internal class MvcViewOptionsSetup : IConfigureOptions<MvcViewOptions>
    {
        private readonly IStringLocalizerFactory _stringLocalizerFactory;
        private readonly IValidationAttributeAdapterFactory _validationAttributeAdapterFactory;

        public MvcViewOptionsSetup(
            IValidationAttributeAdapterFactory validationAttributeAdapterFactory)
        {
            _validationAttributeAdapterFactory = validationAttributeAdapterFactory;
        }

        public MvcViewOptionsSetup(
            IValidationAttributeAdapterFactory validationAttributeAdapterFactory,
            IStringLocalizerFactory stringLocalizerFactory)
            : this(validationAttributeAdapterFactory)
        {
            _stringLocalizerFactory = stringLocalizerFactory;
        }

        public void Configure(MvcViewOptions options)
        {
            options.ClientModelValidatorProviders.Clear();
            options.ClientModelValidatorProviders.Add(new DefaultClientModelValidatorProvider());
            options.ClientModelValidatorProviders.Add(new ClientModelValidatorProvider(_validationAttributeAdapterFactory, _stringLocalizerFactory));
            options.ClientModelValidatorProviders.Add(new NumericClientModelValidatorProvider());
        }
    }
}
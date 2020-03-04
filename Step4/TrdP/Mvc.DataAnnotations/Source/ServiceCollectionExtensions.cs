using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using TrdP.Localization;
using TrdP.Mvc.DataAnnotations.Localization.ValidationProviders;

namespace TrdP.Mvc.DataAnnotations.Localization
{
    public static class ServiceCollectionExtensions
    {
        public static void AddDataAnnotationsLocalizationProvider<TProviderLocator>(
            this IServiceCollection services,
            Action<DataAnnotationsLocalizationOptions> setupAction = null)
            where TProviderLocator : class
        {
            services.AddLocalizationProvider<TProviderLocator>();

            services.TryAddSingleton<IValidationAttributeAdapterProvider, ValidationAttributeAdapterProvider>();
            services.TryAddEnumerable(ServiceDescriptor.Transient<IConfigureOptions<MvcOptions>, DataAnnotationsMvcOptionsSetup>());
            services.TryAddEnumerable(ServiceDescriptor.Transient<IConfigureOptions<MvcViewOptions>, DataAnnotationsMvcViewOptionsSetup>());
        }
    }
}
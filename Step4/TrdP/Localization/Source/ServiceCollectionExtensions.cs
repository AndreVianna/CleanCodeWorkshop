using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using TrdP.Localization.Abstractions;

namespace TrdP.Localization
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddLocalizationProvider<TProviderLocator>(this IServiceCollection services, Action<LocalizationProviderOptions> setupAction = null)
            where TProviderLocator : class
        {
            services.TryAddSingleton<IStringLocalizerFactory, StringLocalizerFactory<TProviderLocator>>();
            services.TryAddTransient(typeof(IStringLocalizer<>), typeof(StringLocalizer<>));
            services.Configure<LocalizationProviderOptions>(opt => setupAction?.Invoke(opt));
            return services;
        }
    }
}
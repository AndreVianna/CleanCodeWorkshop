using System;
using System.Globalization;
using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using TrdP.Localization;
using TrdP.Mvc.Localization.Abstractions;

namespace TrdP.Mvc.Localization
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddMvcLocalizationProvider<TProviderLocator>(this IServiceCollection services, Action<LocalizerProviderOptions> setupAction = null)
            where TProviderLocator : class
        {
            services.AddLocalizationProvider<TProviderLocator>(setupAction);

            services.TryAddTransient(typeof(IHtmlLocalizerFactory), typeof(HtmlLocalizerFactory));
            services.TryAddTransient(typeof(IHtmlLocalizer<>), typeof(HtmlLocalizer<>));
            services.TryAddTransient(typeof(IViewLocalizer), typeof(ViewLocalizer));
            SetRequestLocalizationOptions(services, setupAction);

            return services;
        }

        private static void SetRequestLocalizationOptions(IServiceCollection services, Action<LocalizerProviderOptions> setupAction)
        {
            var options = new LocalizerProviderOptions();
            setupAction?.Invoke(options);
            var availableCultures = options.AvailableCultures.ToList();
            var defaultRequestCulture = (availableCultures.FirstOrDefault() ?? CultureInfo.CurrentUICulture);
            services.Configure<RequestLocalizationOptions>(o =>
            {
                o.DefaultRequestCulture = new RequestCulture(defaultRequestCulture.Name);
                o.SupportedCultures = availableCultures;
                o.SupportedUICultures = availableCultures;
            });
        }
    }
}
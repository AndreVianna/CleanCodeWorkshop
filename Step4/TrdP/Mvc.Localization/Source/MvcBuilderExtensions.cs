using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using TrdP.Localization;
using TrdP.Mvc.Localization.Abstractions;
using static TrdP.Mvc.DataAnnotations.Localization.DataAnnotationsLocalizationServices;

namespace TrdP.Mvc.Localization
{
    public static class MvcBuilderExtensions
    {
        public static IMvcBuilder AddLocalizationProvider<TProviderLocator>(this IMvcBuilder builder, Action<LocalizationProviderOptions> setupAction = null)
            where TProviderLocator : class
        {
            ConfigureServices<TProviderLocator>(builder.Services, setupAction);
            return builder;
        }

        private static void ConfigureServices<TProviderLocator>(IServiceCollection services, Action<LocalizationProviderOptions> setupAction = null)
            where TProviderLocator : class
        {
            services.AddLocalizationProvider<TProviderLocator>(setupAction);

            services.TryAddTransient(typeof(IHtmlLocalizerFactory), typeof(HtmlLocalizerFactory));
            services.TryAddTransient(typeof(IHtmlLocalizer<>), typeof(HtmlLocalizer<>));
            services.TryAddTransient(typeof(IViewLocalizer), typeof(ViewLocalizer));
            services.Configure<LocalizationProviderOptions>(options => setupAction?.Invoke(options));

            SetRequestLocalizationOptions(services, setupAction);
            SetDataAnnotationsLocalizationServices(services);
        }

        private static void SetRequestLocalizationOptions(IServiceCollection services, Action<LocalizationProviderOptions> setupAction)
        {
            var options = new LocalizationProviderOptions();
            setupAction?.Invoke(options);
            services.Configure<RequestLocalizationOptions>(o =>
            {
                o.DefaultRequestCulture = new RequestCulture(options.DefaultUiCulture);
                o.SupportedCultures = options.AvailableCultures;
                o.SupportedUICultures = options.AvailableCultures;
            });
        }
    }
}
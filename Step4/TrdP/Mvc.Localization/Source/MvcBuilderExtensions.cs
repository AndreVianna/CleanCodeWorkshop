using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using TrdP.Mvc.Localization.Abstractions;
using static TrdP.Mvc.DataAnnotations.Localization.DataAnnotationsLocalizationServices;

namespace TrdP.Mvc.Localization
{
    public static class MvcBuilderExtensions
    {
        public static IMvcBuilder AddLocalization(this IMvcBuilder builder, Action<MvcLocalizationOptions> setupAction = null)
        {
            ConfigureServices(builder.Services, setupAction);
            return builder;
        }

        private static void ConfigureServices(IServiceCollection services, Action<MvcLocalizationOptions> setupAction = null)
        {
            services.TryAddTransient(typeof(IHtmlLocalizerFactory), typeof(HtmlLocalizerFactory));
            services.TryAddTransient(typeof(IHtmlLocalizer<>), typeof(HtmlLocalizer<>));
            services.TryAddTransient(typeof(IViewLocalizer), typeof(ViewLocalizer));
            services.Configure<MvcLocalizationOptions>(options => setupAction?.Invoke(options));
            SetDataAnnotationsLocalizationServices(services);
            SetRequestLocalizationOptions(services);
        }

        private static void SetRequestLocalizationOptions(IServiceCollection services)
        {
            services.PostConfigure<RequestLocalizationOptions>(options =>
            {
                var provider = services.BuildServiceProvider();
                var mvcLocalizationOptions = provider.GetService<IOptions<MvcLocalizationOptions>>();
                options.DefaultRequestCulture = new RequestCulture(mvcLocalizationOptions.Value.DefaultUiCulture);
                options.SupportedCultures = mvcLocalizationOptions.Value.AvailableCultures;
                options.SupportedUICultures = mvcLocalizationOptions.Value.AvailableCultures;
            });
        }
    }
}
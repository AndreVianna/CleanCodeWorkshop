using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.DependencyInjection;

namespace XPenC.WebApp.Localization.Extensions
{
    [ExcludeFromCodeCoverage]
    public static class MvcBuilderExtensions
    {
        public static IMvcBuilder AddLocalizedResources(this IMvcBuilder mvcBuilder)
        {
            mvcBuilder.Services.AddLocalization(options => options.ResourcesPath = typeof(Resources).Name);

            mvcBuilder
                .AddDataAnnotationsLocalization(options => {
                    options.DataAnnotationLocalizerProvider = (type, factory) =>
                        factory.Create(typeof(Models));
                });

            var supportedCultures = Resources.SupportedCultures;
            mvcBuilder.Services.Configure<RequestLocalizationOptions>(options =>
            {
                options.DefaultRequestCulture = new RequestCulture(supportedCultures[0].Name);
                options.SupportedCultures = supportedCultures;
                options.SupportedUICultures = supportedCultures;
            });
            return mvcBuilder;
        }
    }
}

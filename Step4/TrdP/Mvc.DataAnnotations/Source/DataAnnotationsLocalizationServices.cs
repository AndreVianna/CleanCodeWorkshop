using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using TrdP.Mvc.DataAnnotations.Localization.ValidationProviders;

namespace TrdP.Mvc.DataAnnotations.Localization
{
    public static class DataAnnotationsLocalizationServices
    {
        public static void SetDataAnnotationsLocalizationServices(IServiceCollection services)
        {
            services.TryAddSingleton<IValidationAttributeAdapterFactory, ValidationAttributeAdapterFactory>();
            services.TryAddEnumerable(ServiceDescriptor.Transient<IConfigureOptions<MvcOptions>, MvcOptionsSetup>());
            services.TryAddEnumerable(ServiceDescriptor.Transient<IConfigureOptions<MvcViewOptions>, MvcViewOptionsSetup>());
        }
    }
}
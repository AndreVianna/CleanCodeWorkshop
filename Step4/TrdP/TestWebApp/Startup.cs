using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using TrdP.Localization;
using TrdP.TestWebAppResources;

namespace TrdP.TestWebApp
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1052:Static holder types should be Static or NotInheritable", Justification = "Called at runtime")]
    public class Startup
    {
        public static void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();

            services.AddLocalizationProvider<ResourcesProvider>();

            var supportedCultures = ResourcesProvider.SupportedCultures;
            services.Configure<RequestLocalizationOptions>(options =>
            {
                options.DefaultRequestCulture = new RequestCulture(supportedCultures[0].Name);
                options.SupportedCultures = supportedCultures;
                options.SupportedUICultures = supportedCultures;
            });
        }

        // ReSharper disable once UnusedMember.Global - Called at runtime.
        public static void Configure(IApplicationBuilder app)
        {
            var locOptions = app.ApplicationServices.GetService<IOptions<RequestLocalizationOptions>>();
            app.UseRequestLocalization(locOptions.Value);

            app.UseDeveloperExceptionPage();
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();
            });
        }
    }
}

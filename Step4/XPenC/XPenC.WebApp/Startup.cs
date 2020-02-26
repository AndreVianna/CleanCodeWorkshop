using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using XPenC.BusinessLogic.Extensions;
using XPenC.DataAccess.EntityFrameworkCore.Extensions;
using XPenC.WebApp.Configuration;
using XPenC.WebApp.Localization;

namespace XPenC.WebApp
{
    [ExcludeFromCodeCoverage]
    public class Startup
    {
        private readonly IConfiguration _configuration;

        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        //The first supported culture is considered the default one.
        private readonly IList<CultureInfo> _supportedCultures = new List<CultureInfo>
        {
            new CultureInfo("en-US"),
            new CultureInfo("pt-Br"),
        };

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddLocalizedMvc<Resources, Localization.Models>(_supportedCultures);

            services.AddEntityFrameworkDataContext(_configuration, "DataContext");

            services.AddBusinessServices();
        }

        // ReSharper disable once UnusedMember.Global - This method gets called by the runtime.
        public static void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            var locOptions = app.ApplicationServices.GetService<IOptions<RequestLocalizationOptions>>();
            app.UseRequestLocalization(locOptions.Value);

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }
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

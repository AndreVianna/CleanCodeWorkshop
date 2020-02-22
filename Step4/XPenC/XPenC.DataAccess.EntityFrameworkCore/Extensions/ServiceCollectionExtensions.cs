using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using XPenC.DataAccess.Contracts;

namespace XPenC.DataAccess.EntityFrameworkCore.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddEntityFrameworkDataContext(this IServiceCollection services, IConfiguration configuration, string connectionStringName)
        {
            services.AddDbContext<XPenCDbContext>(options => options.UseSqlServer(configuration[$"ConnectionStrings:{connectionStringName}"]));
            services.AddScoped<IDataContext, EntityFrameworkDataContext>();
            return services;
        }
    }
}

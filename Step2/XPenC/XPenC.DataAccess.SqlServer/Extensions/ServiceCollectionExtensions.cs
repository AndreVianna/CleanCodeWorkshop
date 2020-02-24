using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using XPenC.DataAccess.Contracts;

namespace XPenC.DataAccess.SqlServer.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddSqlServerDataContext(this IServiceCollection services, IConfiguration configuration, string connectionStringName)
        {
            services.AddScoped<ISqlDataProvider>(provider => new SqlDataProvider(configuration, connectionStringName));
            services.AddScoped<IDataContext, SqlServerDataContext>();
            return services;
        }
    }
}

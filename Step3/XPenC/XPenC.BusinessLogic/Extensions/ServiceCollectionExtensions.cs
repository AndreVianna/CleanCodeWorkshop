using Microsoft.Extensions.DependencyInjection;
using XPenC.BusinessLogic.Contracts;

namespace XPenC.BusinessLogic.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddBusinessServices(this IServiceCollection services)
        {
            services.AddScoped<IExpenseReportOperations, ExpenseReportOperations>();
            return services;
        }
    }
}

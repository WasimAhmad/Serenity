using Microsoft.Extensions.DependencyInjection;

namespace Serenity.Workflow
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddSerenityWorkflow(this IServiceCollection services)
        {
            services.AddScoped<WorkflowEngine>();
            return services;
        }
    }
}

using Microsoft.Extensions.DependencyInjection;

namespace Serenity.Workflow
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddSerenityWorkflow(this IServiceCollection services)
        {
            services.AddSingleton<WorkflowEngine>();
            services.AddSingleton<IWorkflowHistoryStore, InMemoryWorkflowHistoryStore>();
            return services;
        }
    }
}

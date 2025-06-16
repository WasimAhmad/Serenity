using Microsoft.Extensions.DependencyInjection;
using Serene.Web.Workflow.Abstractions;

namespace Serene.Web.Workflow.Core
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddSerenityWorkflow(this IServiceCollection services,
            Action<WorkflowEngineOptions>? configure = null)
        {
            var options = new WorkflowEngineOptions();
            configure?.Invoke(options);
            services.AddSingleton(options);
            services.AddScoped<WorkflowEngine>();
            if (options.UseInMemoryHistoryStore)
                services.AddSingleton<IWorkflowHistoryStore, InMemoryWorkflowHistoryStore>();
            return services;
        }
    }
}

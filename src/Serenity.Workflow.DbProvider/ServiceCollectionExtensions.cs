using Microsoft.Extensions.DependencyInjection;
using Serenity.Workflow.Provider;

namespace Serenity.Workflow
{
    public static class DbProviderServiceCollectionExtensions
    {
        public static IServiceCollection AddWorkflowDbProvider(this IServiceCollection services)
        {
            services.AddScoped<IWorkflowDefinitionProvider, DatabaseWorkflowDefinitionProvider>();
            return services;
        }
    }
}

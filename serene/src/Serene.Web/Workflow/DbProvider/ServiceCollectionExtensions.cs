using Microsoft.Extensions.DependencyInjection;
using Serene.Web.Workflow.Abstractions;
using Serene.Web.Workflow.DbProvider.Provider;
using Serene.Web.Workflow.DbProvider.Store;

namespace Serene.Web.Workflow.DbProvider
{
    public static class DbProviderServiceCollectionExtensions
    {
        public static IServiceCollection AddWorkflowDbProvider(this IServiceCollection services)
        {
            services.AddScoped<IWorkflowDefinitionProvider, DatabaseWorkflowDefinitionProvider>();
            services.AddScoped<IWorkflowHistoryStore, DBWorkflowHistoryStore>();
            return services;
        }
    }
}

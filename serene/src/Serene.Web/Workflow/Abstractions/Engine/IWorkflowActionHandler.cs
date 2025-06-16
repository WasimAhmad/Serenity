using System.Threading.Tasks;

namespace Serene.Web.Workflow.Abstractions
{
    public interface IWorkflowActionHandler
    {
        Task ExecuteAsync(IServiceProvider services, object instance, IDictionary<string, object?>? input);
    }
}

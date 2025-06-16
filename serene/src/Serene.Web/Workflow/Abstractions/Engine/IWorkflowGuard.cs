using System.Threading.Tasks;

namespace Serene.Web.Workflow.Abstractions
{
    public interface IWorkflowGuard
    {
        Task<bool> CanExecuteAsync(IServiceProvider services, object instance);
    }
}

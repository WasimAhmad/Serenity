using System.Threading.Tasks;

namespace Serenity.Workflow
{
    public interface IWorkflowGuard
    {
        Task<bool> CanExecuteAsync(IServiceProvider services, object instance);
    }
}

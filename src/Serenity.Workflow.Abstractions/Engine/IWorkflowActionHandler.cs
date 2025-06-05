using System.Threading.Tasks;

namespace Serenity.Workflow
{
    public interface IWorkflowActionHandler
    {
        Task ExecuteAsync(IServiceProvider services, object instance, IDictionary<string, object?>? input);
    }
}

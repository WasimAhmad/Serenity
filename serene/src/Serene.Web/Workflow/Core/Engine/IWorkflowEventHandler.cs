using System.Collections.Generic;
using System.Threading.Tasks;

namespace Serene.Web.Workflow.Core
{
    public interface IWorkflowEventHandler
    {
        Task OnTriggerFiredAsync(IServiceProvider services, WorkflowEngine engine,
            string workflowKey, string fromState, string trigger,
            IDictionary<string, object?>? input);

        Task OnExitStateAsync(IServiceProvider services, WorkflowEngine engine,
            string workflowKey, string state);

        Task OnEnterStateAsync(IServiceProvider services, WorkflowEngine engine,
            string workflowKey, string state);
    }
}

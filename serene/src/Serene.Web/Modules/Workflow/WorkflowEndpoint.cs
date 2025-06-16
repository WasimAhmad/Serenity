using Microsoft.AspNetCore.Mvc;
using Serenity.Services;
using Serene.Web.Workflow.Abstractions;
using Serene.Web.Workflow.Core;
using System.Linq;
using System.Threading.Tasks;

namespace Serene.Workflow
{
    [Route("Services/Workflow/[action]")]
    public class WorkflowEndpoint : ServiceEndpoint
    {
        [HttpPost]
        public async Task<ServiceResponse> ExecuteAction(ExecuteWorkflowActionRequest request, [FromServices] WorkflowEngine engine)
        {
            await engine.ExecuteAsync(request.WorkflowKey, request.CurrentState, request.Trigger, request.Input);
            return new ServiceResponse();
        }

        [HttpPost]
        public GetPermittedActionsResponse GetPermittedActions(GetPermittedActionsRequest request, [FromServices] WorkflowEngine engine)
        {
            var actions = engine.GetPermittedTriggers(request.WorkflowKey, request.CurrentState);
            return new GetPermittedActionsResponse { Actions = actions.ToList() };
        }

        [HttpPost]
        public GetWorkflowDefinitionResponse GetDefinition(GetWorkflowDefinitionRequest request,
            [FromServices] IWorkflowDefinitionProvider provider)
        {
            var def = provider.GetDefinition(request.WorkflowKey);
            return new GetWorkflowDefinitionResponse { Definition = def };
        }

        [HttpPost]
        public GetWorkflowHistoryResponse GetHistory(GetWorkflowHistoryRequest request,
            [FromServices] IWorkflowHistoryStore history)
        {
            var list = history.GetHistory(request.WorkflowKey, request.EntityId);
            return new GetWorkflowHistoryResponse { History = list.ToList() };
        }
    }
}

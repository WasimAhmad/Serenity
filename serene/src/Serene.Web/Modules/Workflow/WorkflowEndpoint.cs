using Microsoft.AspNetCore.Mvc;
using Serenity.Services;
using Serenity.Workflow;
using System.Linq;

namespace Serene.Workflow
{
    [Route("Services/Workflow/[action]")]
    public class WorkflowEndpoint : ServiceEndpoint
    {
        [HttpPost]
        public ServiceResponse ExecuteAction(ExecuteWorkflowActionRequest request, [FromServices] WorkflowEngine engine)
        {
            engine.ExecuteAsync(request.WorkflowKey, request.CurrentState, request.Trigger, request.Input);
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

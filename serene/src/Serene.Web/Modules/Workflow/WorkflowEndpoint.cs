using Microsoft.AspNetCore.Mvc;
using Serenity.Services;
using Serenity.Workflow;

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
    }
}

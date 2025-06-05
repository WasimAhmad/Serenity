using Serenity.Services;

namespace Serenity.Workflow
{
    public class GetPermittedActionsRequest : ServiceRequest
    {
        public required string WorkflowKey { get; set; }
        public required string CurrentState { get; set; }
    }
}

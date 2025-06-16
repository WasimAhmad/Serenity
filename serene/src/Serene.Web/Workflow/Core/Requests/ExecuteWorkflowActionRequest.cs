using Serenity.Services;

namespace Serene.Web.Workflow.Core
{
    public class ExecuteWorkflowActionRequest : ServiceRequest
    {
        public required string WorkflowKey { get; set; }
        public required string CurrentState { get; set; }
        public required string Trigger { get; set; }
        public IDictionary<string, object?>? Input { get; set; }
    }
}

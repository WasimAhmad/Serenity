using Serenity.Services;

namespace Serenity.Workflow
{
    public class GetWorkflowDefinitionRequest : ServiceRequest
    {
        public required string WorkflowKey { get; set; }
    }
}

using Serenity.Services;

namespace Serene.Web.Workflow.Core
{
    public class GetWorkflowDefinitionRequest : ServiceRequest
    {
        public required string WorkflowKey { get; set; }
    }
}

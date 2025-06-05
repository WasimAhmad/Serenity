using Serenity.Services;

namespace Serenity.Workflow
{
    public class GetWorkflowDefinitionResponse : ServiceResponse
    {
        public WorkflowDefinition? Definition { get; set; }
    }
}

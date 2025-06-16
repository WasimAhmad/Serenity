using Serenity.Services;
using Serene.Web.Workflow.Abstractions;

namespace Serene.Web.Workflow.Core
{
    public class GetWorkflowDefinitionResponse : ServiceResponse
    {
        public WorkflowDefinition? Definition { get; set; }
    }
}

namespace Serene.Web.Workflow.Abstractions
{
    public interface IWorkflowDefinitionProvider
    {
        WorkflowDefinition? GetDefinition(string workflowKey);
    }
}

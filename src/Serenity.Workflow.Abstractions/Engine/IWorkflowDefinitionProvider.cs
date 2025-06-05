namespace Serenity.Workflow
{
    public interface IWorkflowDefinitionProvider
    {
        WorkflowDefinition? GetDefinition(string workflowKey);
    }
}

namespace Serene.WorkflowManagement.Forms;

[FormScript("Workflow.WorkflowDefinition")]
[BasedOnRow(typeof(Serenity.Workflow.Entities.WorkflowDefinitionRow), CheckNames = true)]
public class WorkflowDefinitionForm
{
    public string WorkflowKey { get; set; }
    public string Name { get; set; }
    public string InitialState { get; set; }
}

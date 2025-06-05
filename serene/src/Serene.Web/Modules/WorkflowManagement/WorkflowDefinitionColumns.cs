namespace Serene.WorkflowManagement.Forms;

[ColumnsScript("Workflow.WorkflowDefinition")]
[BasedOnRow(typeof(Serenity.Workflow.Entities.WorkflowDefinitionRow), CheckNames = true)]
public class WorkflowDefinitionColumns
{
    [EditLink]
    public string WorkflowKey { get; set; }
    public string Name { get; set; }
    public string InitialState { get; set; }
}

namespace Serene.Tasks.Forms;

[ColumnsScript("Tasks.TaskItem")]
[BasedOnRow(typeof(TaskItemRow), CheckNames = true)]
public class TaskItemColumns
{
    [EditLink]
    public string Title { get; set; }
    public string State { get; set; }
}

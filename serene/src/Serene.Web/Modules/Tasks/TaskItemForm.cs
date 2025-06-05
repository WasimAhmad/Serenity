namespace Serene.Tasks.Forms;

[FormScript("Tasks.TaskItem")]
[BasedOnRow(typeof(TaskItemRow), CheckNames = true)]
public class TaskItemForm
{
    public string Title { get; set; }
    public string State { get; set; }
}

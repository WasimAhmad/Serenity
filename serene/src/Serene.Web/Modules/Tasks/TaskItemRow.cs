using Serene.Web.Workflow.Abstractions;

namespace Serene.Tasks;

[ConnectionKey("Default"), Module("Tasks"), TableName("TaskItems")]
[DisplayName("Task Items"), InstanceName("Task Item")]
[ReadPermission("Task:View")]
[ModifyPermission("Task:Modify")]
[WorkflowEnabled("TaskWorkflow")]
public sealed class TaskItemRow : Row<TaskItemRow.RowFields>, IIdRow, INameRow
{
    [DisplayName("Task Id"), Identity, IdProperty]
    public int? TaskId { get => fields.TaskId[this]; set => fields.TaskId[this] = value; }

    [DisplayName("Title"), Size(100), NotNull, QuickSearch, NameProperty]
    public string Title { get => fields.Title[this]; set => fields.Title[this] = value; }

    [DisplayName("State"), Size(50), NotNull]
    [WorkflowStateField]
    public string State { get => fields.State[this]; set => fields.State[this] = value; }

    public class RowFields : RowFieldsBase
    {
        public Int32Field TaskId;
        public StringField Title;
        public StringField State;
    }
}

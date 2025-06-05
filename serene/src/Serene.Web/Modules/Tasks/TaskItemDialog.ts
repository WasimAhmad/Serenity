import { Decorators } from "@serenity-is/corelib";
import { TaskItemRow, TaskItemForm, TaskItemService } from "../ServerTypes/Tasks";
import { WorkflowEntityDialog } from "../Workflow/Client/WorkflowEntityDialog";

@Decorators.registerClass('Serene.Tasks.TaskItemDialog')
export class TaskItemDialog extends WorkflowEntityDialog<TaskItemRow, any> {
    protected getFormKey() { return TaskItemForm.formKey; }
    protected getIdProperty() { return TaskItemRow.idProperty; }
    protected getLocalTextPrefix() { return TaskItemRow.localTextPrefix; }
    protected getNameProperty() { return TaskItemRow.nameProperty; }
    protected getService() { return TaskItemService.baseUrl; }

    protected form = new TaskItemForm(this.idPrefix);

    protected getWorkflowKey() { return 'TaskWorkflow'; }
    protected getStateProperty(): keyof TaskItemRow { return 'State'; }
}

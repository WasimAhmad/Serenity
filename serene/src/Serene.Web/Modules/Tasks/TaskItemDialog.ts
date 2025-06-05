import { Decorators, EntityDialog } from "@serenity-is/corelib";
import { TaskItemRow, TaskItemForm, TaskItemService } from "../ServerTypes/Tasks";
import { WorkflowService } from "../Workflow/Client/WorkflowService";

const startBtn = 'start-workflow';
const finishBtn = 'finish-workflow';

@Decorators.registerClass('Serene.Tasks.TaskItemDialog')
export class TaskItemDialog extends EntityDialog<TaskItemRow, any> {
    protected getFormKey() { return TaskItemForm.formKey; }
    protected getIdProperty() { return TaskItemRow.idProperty; }
    protected getLocalTextPrefix() { return TaskItemRow.localTextPrefix; }
    protected getNameProperty() { return TaskItemRow.nameProperty; }
    protected getService() { return TaskItemService.baseUrl; }

    protected form = new TaskItemForm(this.idPrefix);

    protected getToolbarButtons() {
        let buttons = super.getToolbarButtons();

        buttons.push({
            title: 'Start',
            cssClass: startBtn,
            icon: 'fa-play text-purple',
            onClick: () => this.executeAction('Start')
        });

        buttons.push({
            title: 'Finish',
            cssClass: finishBtn,
            icon: 'fa-flag-checkered text-purple',
            onClick: () => this.executeAction('Finish')
        });

        return buttons;
    }

    private executeAction(trigger: string) {
        WorkflowService.ExecuteAction({
            WorkflowKey: 'TaskWorkflow',
            CurrentState: this.entity.State ?? '',
            Trigger: trigger,
            Input: { EntityId: this.entity.TaskId }
        }).then(() => this.loadById(this.entity.TaskId));
    }

    protected updateInterface() {
        super.updateInterface();

        const start = this.toolbar.findButton(startBtn);
        const finish = this.toolbar.findButton(finishBtn);
        if (this.isNewOrDeleted()) {
            start.addClass('disabled');
            finish.addClass('disabled');
            return;
        }

        WorkflowService.GetPermittedActions({
            WorkflowKey: 'TaskWorkflow',
            CurrentState: this.entity.State ?? ''
        }).then(r => {
            start.toggleClass('disabled', r.Actions.indexOf('Start') < 0);
            finish.toggleClass('disabled', r.Actions.indexOf('Finish') < 0);
        });
    }
}

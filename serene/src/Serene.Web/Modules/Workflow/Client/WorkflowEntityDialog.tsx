import { EntityDialog, ToolButton, Decorators, PropertyDialog } from "@serenity-is/corelib";
import { WorkflowService, WorkflowDefinition } from "./WorkflowService";
import { WorkflowHistoryDialog } from "./WorkflowHistoryDialog";

@Decorators.registerClass('Serene.Workflow.WorkflowEntityDialog')
export abstract class WorkflowEntityDialog<TItem, TOptions> extends EntityDialog<TItem, TOptions> {

    protected abstract getWorkflowKey(): string;
    protected abstract getStateProperty(): keyof TItem;

    private workflow?: WorkflowDefinition;
    private workflowGroup?: HTMLElement;

    protected async ensureDefinition() {
        if (this.workflow)
            return;
        const r = await WorkflowService.GetDefinition({ WorkflowKey: this.getWorkflowKey() });
        this.workflow = r.Definition!;
    }

    protected override getToolbarButtons(): ToolButton[] {
        return super.getToolbarButtons();
    }

    protected override onDialogOpen(): void {
        super.onDialogOpen();
        this.ensureDefinition().then(() => {
            if (!this.workflow || !this.toolbar)
                return;

            let group = this.toolbar.domNode.querySelector('.tool-group');
            if (!group)
                return;

            group = group.parentElement!.appendChild(<div class="tool-group workflow-group" />);
            this.workflowGroup = group as HTMLElement;

            this.toolbar.createButton(group, {
                title: 'History',
                cssClass: 'workflow-history-button',
                icon: 'fa-history text-green',
                separator: 'right',
                onClick: () => this.showHistory()
            });

            this.updateTriggers();
        });
    }

    private executeAction(triggerKey: string) {
        const entity: any = this.entity as any;
        const trigger = this.workflow?.Triggers[triggerKey];

        const doExecute = (input?: any) => {
            WorkflowService.ExecuteAction({
                WorkflowKey: this.getWorkflowKey(),
                CurrentState: entity[this.getStateProperty()] ?? '',
                Trigger: triggerKey,
                Input: { EntityId: entity[this.getIdProperty()], ...input }
            }).then(() => this.loadById(entity[this.getIdProperty()]));
        };

        if (trigger?.RequiresInput && trigger.FormKey) {
            class TriggerDialog extends PropertyDialog<any, any> {
                protected override getFormKey() { return trigger.FormKey; }
            }
            const dlg = new TriggerDialog(trigger.FormKey);
            dlg.dialogTitle = trigger.DisplayName || trigger.TriggerKey;
            dlg.dialogOpen();
            dlg.onClose((r: string) => {
                if (r === 'ok')
                    doExecute((dlg as any).getSaveEntity());
            });
        } else {
            doExecute();
        }
    }

    private showHistory() {
        const entity: any = this.entity as any;
        (new (WorkflowHistoryDialog as any))
            .loadAndOpenDialog({
                WorkflowKey: this.getWorkflowKey(),
                EntityId: entity[this.getIdProperty()]
            });
    }

    private updateTriggers() {
        if (!this.workflow || !this.workflowGroup)
            return;

        const entity: any = this.entity as any;
        WorkflowService.GetPermittedActions({
            WorkflowKey: this.getWorkflowKey(),
            CurrentState: entity[this.getStateProperty()] ?? ''
        }).then(r => {
            this.workflowGroup!.querySelectorAll('div.tool-button.trigger-button').forEach(el => el.remove());
            if (this.isNewOrDeleted())
                return;

            for (const key of r.Actions ?? []) {
                const trigger = this.workflow!.Triggers[key];
                if (!trigger)
                    continue;
                this.toolbar.createButton(this.workflowGroup!, {
                    title: trigger.DisplayName || trigger.TriggerKey,
                    cssClass: `trigger-${trigger.TriggerKey} trigger-button`,
                    icon: 'fa-play text-purple',
                    onClick: () => this.executeAction(trigger.TriggerKey)
                });
            }
        });
    }

    protected override updateInterface() {
        super.updateInterface();
        this.updateTriggers();
    }
}

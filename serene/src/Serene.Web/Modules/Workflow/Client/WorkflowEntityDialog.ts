import { EntityDialog, ToolButton, Decorators } from "@serenity-is/corelib";
import { WorkflowService, WorkflowDefinition } from "./WorkflowService";

@Decorators.registerClass('Serene.Workflow.WorkflowEntityDialog')
export abstract class WorkflowEntityDialog<TItem, TOptions> extends EntityDialog<TItem, TOptions> {

    protected abstract getWorkflowKey(): string;
    protected abstract getStateProperty(): keyof TItem;

    private workflow?: WorkflowDefinition;

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

            for (const key of Object.keys(this.workflow.Triggers)) {
                const trigger = this.workflow.Triggers[key];
                this.toolbar.createButton(group, {
                    title: trigger.DisplayName || trigger.TriggerKey,
                    cssClass: `trigger-${trigger.TriggerKey}`,
                    icon: 'fa-play text-purple',
                    onClick: () => this.executeAction(trigger.TriggerKey)
                });
            }
            this.updateInterface();
        });
    }

    private executeAction(trigger: string) {
        const entity: any = this.entity as any;
        WorkflowService.ExecuteAction({
            WorkflowKey: this.getWorkflowKey(),
            CurrentState: entity[this.getStateProperty()] ?? '',
            Trigger: trigger,
            Input: { EntityId: entity[this.getIdProperty()] }
        }).then(() => this.loadById(entity[this.getIdProperty()]));
    }

    protected override updateInterface() {
        super.updateInterface();
        const entity: any = this.entity as any;
        if (!this.workflow)
            return;
        WorkflowService.GetPermittedActions({
            WorkflowKey: this.getWorkflowKey(),
            CurrentState: entity[this.getStateProperty()] ?? ''
        }).then(r => {
            for (const key of Object.keys(this.workflow!.Triggers)) {
                const btn = this.toolbar.findButton(`trigger-${key}`);
                btn.toggleClass('disabled', r.Actions.indexOf(key) < 0 || this.isNewOrDeleted());
            }
        });
    }
}

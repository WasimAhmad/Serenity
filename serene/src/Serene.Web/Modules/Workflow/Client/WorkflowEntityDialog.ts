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

    protected override async getToolbarButtons(): Promise<ToolButton[]> {
        const buttons = super.getToolbarButtons();
        await this.ensureDefinition();
        if (this.workflow) {
            for (const key of Object.keys(this.workflow.Triggers)) {
                const trigger = this.workflow.Triggers[key];
                buttons.push({
                    title: trigger.DisplayName || trigger.TriggerKey,
                    cssClass: `trigger-${trigger.TriggerKey}`,
                    icon: 'fa-play text-purple',
                    onClick: () => this.executeAction(trigger.TriggerKey)
                });
            }
        }
        return buttons;
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

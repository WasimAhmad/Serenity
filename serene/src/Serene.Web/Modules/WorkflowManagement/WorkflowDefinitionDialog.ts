import { Decorators, EntityDialog } from "@serenity-is/corelib";
import { WorkflowDefinitionRow, WorkflowDefinitionForm, WorkflowDefinitionService } from "../ServerTypes/Workflow";

@Decorators.registerClass('Serene.WorkflowManagement.WorkflowDefinitionDialog')
export class WorkflowDefinitionDialog extends EntityDialog<WorkflowDefinitionRow, any> {
    protected getFormKey() { return WorkflowDefinitionForm.formKey; }
    protected getIdProperty() { return WorkflowDefinitionRow.idProperty; }
    protected getLocalTextPrefix() { return WorkflowDefinitionRow.localTextPrefix; }
    protected getNameProperty() { return WorkflowDefinitionRow.nameProperty; }
    protected getService() { return WorkflowDefinitionService.baseUrl; }

    protected form = new WorkflowDefinitionForm(this.idPrefix);
}

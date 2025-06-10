import { Decorators } from "@serenity-is/corelib";
import { DocumentRow, DocumentForm, DocumentService } from "../ServerTypes/Documents";
import { WorkflowEntityDialog } from "../Workflow/Client/WorkflowEntityDialog";

@Decorators.panel(false)
@Decorators.registerClass('Serene.Documents.DocumentDialog')
export class DocumentDialog extends WorkflowEntityDialog<DocumentRow, any> {
    protected getFormKey() { return DocumentForm.formKey; }
    protected getIdProperty() { return DocumentRow.idProperty; }
    protected getLocalTextPrefix() { return DocumentRow.localTextPrefix; }
    protected getNameProperty() { return DocumentRow.nameProperty; }
    protected getService() { return DocumentService.baseUrl; }

    protected form = new DocumentForm(this.idPrefix);

    protected getWorkflowKey() { return 'DocumentWorkflow'; }
    protected getStateProperty(): keyof DocumentRow { return 'State'; }
}

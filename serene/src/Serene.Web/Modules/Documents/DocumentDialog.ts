import { Decorators } from "@serenity-is/corelib";
import { DocumentRow, DocumentForm, DocumentService,DocumentType } from "../ServerTypes/Documents";
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

    //protected getWorkflowKey() { return 'DocumentWorkflow'; }
    protected getStateProperty(): keyof DocumentRow { return 'State'; }
    constructor() {
        super();
        this.form.DocumentType.changeSelect2(() => {
            this.reloadWorkflow();
        });
    }


    protected getWorkflowKey() {
        switch (this.form.DocumentType.value) {
            case DocumentType.Casual.toString():
                return 'DocumentWorkflow';
            case DocumentType.Annual.toString():
                return 'DocumentWorkflow1';
            default:
                return 'DocumentWorkflow1';
        }
    }


    protected override afterLoadEntity() {
        super.afterLoadEntity();
        this.reloadWorkflow();
    }

}

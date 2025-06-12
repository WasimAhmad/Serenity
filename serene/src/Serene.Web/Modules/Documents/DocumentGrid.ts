import { EntityGrid, Decorators } from "@serenity-is/corelib";
import { DocumentRow, DocumentColumns, DocumentService } from "../ServerTypes/Documents";
import { DocumentDialog } from "./DocumentDialog";
import { WorkflowHistoryGridMixin } from "../Workflow/Client/WorkflowHistoryGridMixin";

@Decorators.registerClass('Serene.Documents.DocumentGrid')
export class DocumentGrid extends EntityGrid<DocumentRow, any> {
    protected useAsync() { return true; }
    protected getColumnsKey() { return DocumentColumns.columnsKey; }
    protected getDialogType() { return DocumentDialog; }
    protected getIdProperty() { return DocumentRow.idProperty; }
    protected getLocalTextPrefix() { return DocumentRow.localTextPrefix; }
    protected getService() { return DocumentService.baseUrl; }

    private history?: WorkflowHistoryGridMixin<DocumentRow>;

    protected override afterInit() {
        super.afterInit();
        this.history = new WorkflowHistoryGridMixin(this, {
            workflowKey: 'DocumentWorkflow',
            idField: DocumentRow.idProperty as keyof DocumentRow
        });
    }
}

import { EntityGrid, Decorators } from "@serenity-is/corelib";
import { WorkflowDefinitionRow, WorkflowDefinitionColumns, WorkflowDefinitionService } from "../ServerTypes/Workflow";
import { WorkflowDefinitionDialog } from "./WorkflowDefinitionDialog";

@Decorators.registerClass('Serene.WorkflowManagement.WorkflowDefinitionGrid')
export class WorkflowDefinitionGrid extends EntityGrid<WorkflowDefinitionRow, any> {
    protected getColumnsKey() { return WorkflowDefinitionColumns.columnsKey; }
    protected getDialogType() { return WorkflowDefinitionDialog; }
    protected getIdProperty() { return WorkflowDefinitionRow.idProperty; }
    protected getLocalTextPrefix() { return WorkflowDefinitionRow.localTextPrefix; }
    protected getService() { return WorkflowDefinitionService.baseUrl; }
}

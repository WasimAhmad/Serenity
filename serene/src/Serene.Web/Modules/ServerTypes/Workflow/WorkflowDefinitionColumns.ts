import { ColumnsBase, fieldsProxy } from "@serenity-is/corelib";
import { Column } from "@serenity-is/sleekgrid";
import { WorkflowDefinitionRow } from "./WorkflowDefinitionRow";

export interface WorkflowDefinitionColumns {
    WorkflowKey: Column<WorkflowDefinitionRow>;
    Name: Column<WorkflowDefinitionRow>;
    InitialState: Column<WorkflowDefinitionRow>;
}

export class WorkflowDefinitionColumns extends ColumnsBase<WorkflowDefinitionRow> {
    static readonly columnsKey = 'Workflow.WorkflowDefinition';
    static readonly Fields = fieldsProxy<WorkflowDefinitionColumns>();
}

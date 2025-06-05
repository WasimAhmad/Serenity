import { fieldsProxy } from "@serenity-is/corelib";

export interface WorkflowDefinitionRow {
    Id?: number;
    WorkflowKey?: string;
    Name?: string;
    InitialState?: string;
}

export abstract class WorkflowDefinitionRow {
    static readonly idProperty = 'Id';
    static readonly nameProperty = 'WorkflowKey';
    static readonly localTextPrefix = 'Workflow.WorkflowDefinition';
    static readonly deletePermission = 'Workflow:Modify';
    static readonly insertPermission = 'Workflow:Modify';
    static readonly readPermission = 'Workflow:View';
    static readonly updatePermission = 'Workflow:Modify';

    static readonly Fields = fieldsProxy<WorkflowDefinitionRow>();
}

import { fieldsProxy } from "@serenity-is/corelib";

export interface WorkflowStateRow {
    Id?: number;
    DefinitionId?: number;
    StateKey?: string;
    Name?: string;
}

export abstract class WorkflowStateRow {
    static readonly idProperty = 'Id';
    static readonly localTextPrefix = 'Web.Workflow.DbProvider.WorkflowState';
    static readonly deletePermission = null;
    static readonly insertPermission = null;
    static readonly readPermission = '';
    static readonly updatePermission = null;

    static readonly Fields = fieldsProxy<WorkflowStateRow>();
}
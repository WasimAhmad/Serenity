import { fieldsProxy } from "@serenity-is/corelib";

export interface WorkflowTransitionRow {
    Id?: number;
    DefinitionId?: number;
    FromState?: string;
    ToState?: string;
    TriggerKey?: string;
    GuardKey?: string;
}

export abstract class WorkflowTransitionRow {
    static readonly idProperty = 'Id';
    static readonly localTextPrefix = 'Web.Workflow.DbProvider.WorkflowTransition';
    static readonly deletePermission = null;
    static readonly insertPermission = null;
    static readonly readPermission = '';
    static readonly updatePermission = null;

    static readonly Fields = fieldsProxy<WorkflowTransitionRow>();
}
import { fieldsProxy } from "@serenity-is/corelib";

export interface WorkflowTriggerRow {
    Id?: number;
    DefinitionId?: number;
    TriggerKey?: string;
    Name?: string;
    HandlerKey?: string;
    RequiresInput?: boolean;
    FormKey?: string;
}

export abstract class WorkflowTriggerRow {
    static readonly idProperty = 'Id';
    static readonly localTextPrefix = 'Web.Workflow.DbProvider.WorkflowTrigger';
    static readonly deletePermission = null;
    static readonly insertPermission = null;
    static readonly readPermission = '';
    static readonly updatePermission = null;

    static readonly Fields = fieldsProxy<WorkflowTriggerRow>();
}
import { fieldsProxy } from "@serenity-is/corelib";

export interface WorkflowHistoryRow {
    Id?: number;
    WorkflowKey?: string;
    EntityId?: string;
    FromState?: string;
    ToState?: string;
    Trigger?: string;
    Input?: string;
    EventDate?: string;
    User?: string;
}

export abstract class WorkflowHistoryRow {
    static readonly idProperty = 'Id';
    static readonly localTextPrefix = 'Web.Workflow.DbProvider.WorkflowHistory';
    static readonly deletePermission = null;
    static readonly insertPermission = null;
    static readonly readPermission = '';
    static readonly updatePermission = null;

    static readonly Fields = fieldsProxy<WorkflowHistoryRow>();
}
import { getLookup, getLookupAsync, fieldsProxy } from "@serenity-is/corelib";

export interface TaskItemRow {
    TaskId?: number;
    Title?: string;
    State?: string;
}

export abstract class TaskItemRow {
    static readonly idProperty = 'TaskId';
    static readonly nameProperty = 'Title';
    static readonly localTextPrefix = 'Tasks.TaskItem';
    static readonly lookupKey = 'Tasks.TaskItem';

    /** @deprecated use getLookupAsync instead */
    static getLookup() { return getLookup<TaskItemRow>('Tasks.TaskItem') }
    static async getLookupAsync() { return getLookupAsync<TaskItemRow>('Tasks.TaskItem') }

    static readonly deletePermission = 'Task:Modify';
    static readonly insertPermission = 'Task:Modify';
    static readonly readPermission = 'Task:View';
    static readonly updatePermission = 'Task:Modify';

    static readonly Fields = fieldsProxy<TaskItemRow>();
}

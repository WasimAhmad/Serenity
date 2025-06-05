import { ColumnsBase, fieldsProxy } from "@serenity-is/corelib";
import { Column } from "@serenity-is/sleekgrid";
import { TaskItemRow } from "./TaskItemRow";

export interface TaskItemColumns {
    Title: Column<TaskItemRow>;
    State: Column<TaskItemRow>;
}

export class TaskItemColumns extends ColumnsBase<TaskItemRow> {
    static readonly columnsKey = 'Tasks.TaskItem';
    static readonly Fields = fieldsProxy<TaskItemColumns>();
}

import { EntityGrid, Decorators } from "@serenity-is/corelib";
import { TaskItemRow, TaskItemColumns, TaskItemService } from "../ServerTypes/Tasks";
import { TaskItemDialog } from "./TaskItemDialog";

@Decorators.registerClass('Serene.Tasks.TaskItemGrid')
export class TaskItemGrid extends EntityGrid<TaskItemRow, any> {
    protected useAsync() { return true; }
    protected getColumnsKey() { return TaskItemColumns.columnsKey; }
    protected getDialogType() { return TaskItemDialog; }
    protected getIdProperty() { return TaskItemRow.idProperty; }
    protected getLocalTextPrefix() { return TaskItemRow.localTextPrefix; }
    protected getService() { return TaskItemService.baseUrl; }
}

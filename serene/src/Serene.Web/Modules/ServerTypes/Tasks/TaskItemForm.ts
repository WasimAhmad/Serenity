import { StringEditor, PrefixedContext, initFormType } from "@serenity-is/corelib";

export interface TaskItemForm {
    Title: StringEditor;
    State: StringEditor;
}

export class TaskItemForm extends PrefixedContext {
    static readonly formKey = 'Tasks.TaskItem';
    private static init: boolean;

    constructor(prefix: string) {
        super(prefix);

        if (!TaskItemForm.init)  {
            TaskItemForm.init = true;

            var w0 = StringEditor;

            initFormType(TaskItemForm, [
                'Title', w0,
                'State', w0
            ]);
        }
    }
}

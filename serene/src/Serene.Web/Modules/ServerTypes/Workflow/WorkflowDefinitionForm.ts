import { StringEditor, PrefixedContext, initFormType } from "@serenity-is/corelib";

export interface WorkflowDefinitionForm {
    WorkflowKey: StringEditor;
    Name: StringEditor;
    InitialState: StringEditor;
}

export class WorkflowDefinitionForm extends PrefixedContext {
    static readonly formKey = 'Workflow.WorkflowDefinition';
    private static init: boolean;

    constructor(prefix: string) {
        super(prefix);

        if (!WorkflowDefinitionForm.init) {
            WorkflowDefinitionForm.init = true;

            var w0 = StringEditor;

            initFormType(WorkflowDefinitionForm, [
                'WorkflowKey', w0,
                'Name', w0,
                'InitialState', w0
            ]);
        }
    }
}

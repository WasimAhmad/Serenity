import { StringEditor, PrefixedContext, initFormType } from "@serenity-is/corelib";

export interface DocumentRequestChangesForm {
    Comment: StringEditor;
}

export class DocumentRequestChangesForm extends PrefixedContext {
    static readonly formKey = 'Workflow.DocumentRequestChanges';
    private static init: boolean;

    constructor(prefix: string) {
        super(prefix);

        if (!DocumentRequestChangesForm.init)  {
            DocumentRequestChangesForm.init = true;

            var w0 = StringEditor;

            initFormType(DocumentRequestChangesForm, [
                'Comment', w0
            ]);
        }
    }
}
import { StringEditor, PrefixedContext, initFormType } from "@serenity-is/corelib";

export interface DocumentResubmitForm {
    Comment: StringEditor;
}

export class DocumentResubmitForm extends PrefixedContext {
    static readonly formKey = 'Workflow.DocumentResubmit';
    private static init: boolean;

    constructor(prefix: string) {
        super(prefix);

        if (!DocumentResubmitForm.init)  {
            DocumentResubmitForm.init = true;

            var w0 = StringEditor;

            initFormType(DocumentResubmitForm, [
                'Comment', w0
            ]);
        }
    }
}
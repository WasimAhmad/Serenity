import { StringEditor, PrefixedContext, initFormType } from "@serenity-is/corelib";

export interface DocumentSubmitForm {
    Comment: StringEditor;
}

export class DocumentSubmitForm extends PrefixedContext {
    static readonly formKey = 'Workflow.DocumentSubmit';
    private static init: boolean;

    constructor(prefix: string) {
        super(prefix);

        if (!DocumentSubmitForm.init)  {
            DocumentSubmitForm.init = true;

            var w0 = StringEditor;

            initFormType(DocumentSubmitForm, [
                'Comment', w0
            ]);
        }
    }
}
import { StringEditor, PrefixedContext, initFormType } from "@serenity-is/corelib";

export interface DocumentRejectForm {
    Reason: StringEditor;
}

export class DocumentRejectForm extends PrefixedContext {
    static readonly formKey = 'Workflow.DocumentReject';
    private static init: boolean;

    constructor(prefix: string) {
        super(prefix);

        if (!DocumentRejectForm.init)  {
            DocumentRejectForm.init = true;

            var w0 = StringEditor;

            initFormType(DocumentRejectForm, [
                'Reason', w0
            ]);
        }
    }
}
import { StringEditor, PrefixedContext, initFormType } from "@serenity-is/corelib";

export interface DocumentForm {
    Title: StringEditor;
    State: StringEditor;
}

export class DocumentForm extends PrefixedContext {
    static readonly formKey = 'Documents.Document';
    private static init: boolean;

    constructor(prefix: string) {
        super(prefix);

        if (!DocumentForm.init)  {
            DocumentForm.init = true;

            var w0 = StringEditor;

            initFormType(DocumentForm, [
                'Title', w0,
                'State', w0
            ]);
        }
    }
}

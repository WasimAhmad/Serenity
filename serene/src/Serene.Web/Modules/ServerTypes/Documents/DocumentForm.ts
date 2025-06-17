import { StringEditor, EnumEditor, PrefixedContext, initFormType } from "@serenity-is/corelib";
import { DocumentType } from "./DocumentType";

export interface DocumentForm {
    Title: StringEditor;
    DocumentType: EnumEditor;
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
            var w1 = EnumEditor;

            initFormType(DocumentForm, [
                'Title', w0,
                'DocumentType', w1,
                'State', w0
            ]);
        }
    }
}

[DocumentType]; // referenced types
﻿import { LookupEditor, TextAreaEditor, PrefixedContext, initFormType } from "@serenity-is/corelib";

export interface DocumentSubmitForm {
    Language: LookupEditor;
    Comment: TextAreaEditor;
}

export class DocumentSubmitForm extends PrefixedContext {
    static readonly formKey = 'Workflow.DocumentSubmit';
    private static init: boolean;

    constructor(prefix: string) {
        super(prefix);

        if (!DocumentSubmitForm.init)  {
            DocumentSubmitForm.init = true;

            var w0 = LookupEditor;
            var w1 = TextAreaEditor;

            initFormType(DocumentSubmitForm, [
                'Language', w0,
                'Comment', w1
            ]);
        }
    }
}
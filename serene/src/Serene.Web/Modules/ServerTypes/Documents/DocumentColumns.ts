import { ColumnsBase, fieldsProxy } from "@serenity-is/corelib";
import { Column } from "@serenity-is/sleekgrid";
import { DocumentRow } from "./DocumentRow";
import { DocumentType } from "./DocumentType";

export interface DocumentColumns {
    Title: Column<DocumentRow>;
    DocumentType: Column<DocumentRow>;
    State: Column<DocumentRow>;
}

export class DocumentColumns extends ColumnsBase<DocumentRow> {
    static readonly columnsKey = 'Documents.Document';
    static readonly Fields = fieldsProxy<DocumentColumns>();
}

[DocumentType]; // referenced types
import { ColumnsBase, fieldsProxy } from "@serenity-is/corelib";
import { Column } from "@serenity-is/sleekgrid";
import { DocumentRow } from "./DocumentRow";

export interface DocumentColumns {
    Title: Column<DocumentRow>;
    State: Column<DocumentRow>;
}

export class DocumentColumns extends ColumnsBase<DocumentRow> {
    static readonly columnsKey = 'Documents.Document';
    static readonly Fields = fieldsProxy<DocumentColumns>();
}

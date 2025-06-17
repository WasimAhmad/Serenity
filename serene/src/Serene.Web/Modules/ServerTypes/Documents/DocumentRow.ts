import { fieldsProxy } from "@serenity-is/corelib";
import { DocumentType } from "./DocumentType";

export interface DocumentRow {
    DocumentId?: number;
    Title?: string;
    State?: string;
    DocumentType?: DocumentType;
}

export abstract class DocumentRow {
    static readonly idProperty = 'DocumentId';
    static readonly nameProperty = 'Title';
    static readonly localTextPrefix = 'Documents.Document';
    static readonly deletePermission = 'Document:Modify';
    static readonly insertPermission = 'Document:Modify';
    static readonly readPermission = 'Document:View';
    static readonly updatePermission = 'Document:Modify';

    static readonly Fields = fieldsProxy<DocumentRow>();
}
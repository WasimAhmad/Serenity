import { Decorators } from "@serenity-is/corelib";

export enum DocumentType {
    Public = 1,
    Internal = 2
}
Decorators.registerEnumType(DocumentType, 'Serene.Documents.DocumentType', 'Documents.DocumentType');
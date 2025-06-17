import { Decorators } from "@serenity-is/corelib";

export enum DocumentType {
    Casual = 1,
    Annual = 2
}
Decorators.registerEnumType(DocumentType, 'Serene.Documents.DocumentType', 'Documents.DocumentType');
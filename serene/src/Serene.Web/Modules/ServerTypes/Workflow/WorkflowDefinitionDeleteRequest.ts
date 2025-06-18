import { DeleteRequest } from "@serenity-is/corelib";

export interface WorkflowDefinitionDeleteRequest extends DeleteRequest {
    DefinitionId?: string;
}
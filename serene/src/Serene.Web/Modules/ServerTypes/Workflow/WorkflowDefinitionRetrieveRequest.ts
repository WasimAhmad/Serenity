import { ServiceRequest } from "@serenity-is/corelib";

export interface WorkflowDefinitionRetrieveRequest extends ServiceRequest {
    DefinitionId?: string;
}
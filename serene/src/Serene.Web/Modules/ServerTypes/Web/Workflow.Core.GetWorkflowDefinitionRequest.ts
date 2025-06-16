import { ServiceRequest } from "@serenity-is/corelib";

export interface GetWorkflowDefinitionRequest extends ServiceRequest {
    WorkflowKey?: string;
}
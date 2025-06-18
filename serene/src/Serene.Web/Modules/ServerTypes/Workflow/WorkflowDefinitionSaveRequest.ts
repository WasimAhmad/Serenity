import { ServiceRequest } from "@serenity-is/corelib";
import { ApiWorkflowDefinition } from "./ApiWorkflowDefinition";

export interface WorkflowDefinitionSaveRequest extends ServiceRequest {
    Definition?: ApiWorkflowDefinition;
}
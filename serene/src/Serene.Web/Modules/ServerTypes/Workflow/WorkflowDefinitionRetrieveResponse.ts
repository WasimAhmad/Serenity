import { ServiceResponse } from "@serenity-is/corelib";
import { ApiWorkflowDefinition } from "./ApiWorkflowDefinition";

export interface WorkflowDefinitionRetrieveResponse extends ServiceResponse {
    Definition?: ApiWorkflowDefinition;
}
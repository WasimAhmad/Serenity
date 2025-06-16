import { ServiceResponse } from "@serenity-is/corelib";
import { WorkflowDefinition } from "./Workflow.Abstractions.WorkflowDefinition";

export interface GetWorkflowDefinitionResponse extends ServiceResponse {
    Definition?: WorkflowDefinition;
}
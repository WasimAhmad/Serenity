import { ServiceRequest } from "@serenity-is/corelib";

export interface ExecuteWorkflowActionRequest extends ServiceRequest {
    WorkflowKey?: string;
    CurrentState?: string;
    Trigger?: string;
}
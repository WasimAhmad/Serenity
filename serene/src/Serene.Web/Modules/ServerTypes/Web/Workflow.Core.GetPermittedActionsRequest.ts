import { ServiceRequest } from "@serenity-is/corelib";

export interface GetPermittedActionsRequest extends ServiceRequest {
    WorkflowKey?: string;
    CurrentState?: string;
}
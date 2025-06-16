import { ServiceRequest } from "@serenity-is/corelib";

export interface GetWorkflowHistoryRequest extends ServiceRequest {
    WorkflowKey?: string;
    EntityId?: any;
}
import { serviceRequest, ServiceRequest, ServiceResponse } from "@serenity-is/corelib";

export interface ExecuteWorkflowActionRequest extends ServiceRequest {
    WorkflowKey: string;
    CurrentState: string;
    Trigger: string;
    Input?: any;
}

export interface GetPermittedActionsRequest extends ServiceRequest {
    WorkflowKey: string;
    CurrentState: string;
}

export interface GetPermittedActionsResponse extends ServiceResponse {
    Actions: string[];
}

export namespace WorkflowService {
    export const baseUrl = 'Workflow';

    export function ExecuteAction(request: ExecuteWorkflowActionRequest) {
        return serviceRequest(baseUrl + '/ExecuteAction', request);
    }

    export function GetPermittedActions(request: GetPermittedActionsRequest) {
        return serviceRequest<GetPermittedActionsResponse>(baseUrl + '/GetPermittedActions', request);
    }
}

import { serviceRequest } from "@serenity-is/corelib";

export interface ExecuteWorkflowActionRequest {
    WorkflowKey: string;
    CurrentState: string;
    Trigger: string;
    Input?: any;
}

export interface GetPermittedActionsRequest {
    WorkflowKey: string;
    CurrentState: string;
}

export interface GetPermittedActionsResponse {
    Actions: string[];
}

export namespace WorkflowService {
    export const baseUrl = 'Services/Workflow';

    export function ExecuteAction(request: ExecuteWorkflowActionRequest) {
        return serviceRequest(baseUrl + '/ExecuteAction', request);
    }

    export function GetPermittedActions(request: GetPermittedActionsRequest) {
        return serviceRequest(baseUrl + '/GetPermittedActions', request);
    }
}

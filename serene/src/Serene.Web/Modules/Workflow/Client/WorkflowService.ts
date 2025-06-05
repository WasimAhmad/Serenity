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

export interface GetWorkflowDefinitionRequest extends ServiceRequest {
    WorkflowKey: string;
}

export interface WorkflowTrigger {
    TriggerKey: string;
    DisplayName?: string;
    HandlerKey?: string;
    FormKey?: string;
    RequiresInput?: boolean;
}

export interface WorkflowDefinition {
    WorkflowKey: string;
    InitialState: string;
    States: { [key: string]: { StateKey: string; DisplayName?: string } };
    Triggers: { [key: string]: WorkflowTrigger };
    Transitions: { From: string; Trigger: string; To: string; GuardKey?: string }[];
}

export interface GetWorkflowDefinitionResponse extends ServiceResponse {
    Definition?: WorkflowDefinition;
}

export interface GetWorkflowHistoryRequest extends ServiceRequest {
    WorkflowKey: string;
    EntityId: any;
}

export interface GetWorkflowHistoryResponse extends ServiceResponse {
    History?: {
        WorkflowKey: string;
        EntityId: any;
        FromState: string;
        ToState: string;
        Trigger: string;
        Input?: any;
        EventDate: string;
        User?: string;
    }[];
}

export namespace WorkflowService {
    export const baseUrl = 'Workflow';

    export function ExecuteAction(request: ExecuteWorkflowActionRequest) {
        return serviceRequest(baseUrl + '/ExecuteAction', request);
    }

    export function GetPermittedActions(request: GetPermittedActionsRequest) {
        return serviceRequest<GetPermittedActionsResponse>(baseUrl + '/GetPermittedActions', request);
    }

    export function GetDefinition(request: GetWorkflowDefinitionRequest) {
        return serviceRequest<GetWorkflowDefinitionResponse>(baseUrl + '/GetDefinition', request);
    }

    export function GetHistory(request: GetWorkflowHistoryRequest) {
        return serviceRequest<GetWorkflowHistoryResponse>(baseUrl + '/GetHistory', request);
    }
}

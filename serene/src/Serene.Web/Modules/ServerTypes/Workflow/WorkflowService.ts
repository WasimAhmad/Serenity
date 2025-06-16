import { ServiceResponse, ServiceOptions, serviceRequest } from "@serenity-is/corelib";
import { ExecuteWorkflowActionRequest } from "../Web/Workflow.Core.ExecuteWorkflowActionRequest";
import { GetPermittedActionsRequest } from "../Web/Workflow.Core.GetPermittedActionsRequest";
import { GetPermittedActionsResponse } from "../Web/Workflow.Core.GetPermittedActionsResponse";
import { GetWorkflowDefinitionRequest } from "../Web/Workflow.Core.GetWorkflowDefinitionRequest";
import { GetWorkflowDefinitionResponse } from "../Web/Workflow.Core.GetWorkflowDefinitionResponse";
import { GetWorkflowHistoryRequest } from "../Web/Workflow.Core.GetWorkflowHistoryRequest";
import { GetWorkflowHistoryResponse } from "../Web/Workflow.Core.GetWorkflowHistoryResponse";

export namespace WorkflowService {
    export const baseUrl = 'Workflow';

    export declare function ExecuteAction(request: ExecuteWorkflowActionRequest, onSuccess?: (response: ServiceResponse) => void, opt?: ServiceOptions<any>): PromiseLike<ServiceResponse>;
    export declare function GetPermittedActions(request: GetPermittedActionsRequest, onSuccess?: (response: GetPermittedActionsResponse) => void, opt?: ServiceOptions<any>): PromiseLike<GetPermittedActionsResponse>;
    export declare function GetDefinition(request: GetWorkflowDefinitionRequest, onSuccess?: (response: GetWorkflowDefinitionResponse) => void, opt?: ServiceOptions<any>): PromiseLike<GetWorkflowDefinitionResponse>;
    export declare function GetHistory(request: GetWorkflowHistoryRequest, onSuccess?: (response: GetWorkflowHistoryResponse) => void, opt?: ServiceOptions<any>): PromiseLike<GetWorkflowHistoryResponse>;

    export const Methods = {
        ExecuteAction: "Workflow/ExecuteAction",
        GetPermittedActions: "Workflow/GetPermittedActions",
        GetDefinition: "Workflow/GetDefinition",
        GetHistory: "Workflow/GetHistory"
    } as const;

    [
        'ExecuteAction', 
        'GetPermittedActions', 
        'GetDefinition', 
        'GetHistory'
    ].forEach(x => {
        (<any>WorkflowService)[x] = function (r, s, o) {
            return serviceRequest(baseUrl + '/' + x, r, s, o);
        };
    });
}
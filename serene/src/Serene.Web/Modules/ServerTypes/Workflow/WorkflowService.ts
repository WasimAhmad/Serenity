import { ServiceResponse, ServiceOptions, serviceRequest } from "@serenity-is/corelib";
import { ExecuteWorkflowActionRequest } from "./ExecuteWorkflowActionRequest";
import { GetPermittedActionsRequest } from "./GetPermittedActionsRequest";
import { GetPermittedActionsResponse } from "./GetPermittedActionsResponse";
import { GetWorkflowDefinitionRequest } from "./GetWorkflowDefinitionRequest";
import { GetWorkflowDefinitionResponse } from "./GetWorkflowDefinitionResponse";
import { GetWorkflowHistoryRequest } from "./GetWorkflowHistoryRequest";
import { GetWorkflowHistoryResponse } from "./GetWorkflowHistoryResponse";

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
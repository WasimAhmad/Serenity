import { ServiceOptions, serviceRequest } from "@serenity-is/corelib";
import { Task } from "../System/Threading.Tasks.Task";
import { ExecuteWorkflowActionRequest } from "./ExecuteWorkflowActionRequest";
import { GetPermittedActionsRequest } from "./GetPermittedActionsRequest";
import { GetPermittedActionsResponse } from "./GetPermittedActionsResponse";

export namespace WorkflowService {
    export const baseUrl = 'Workflow';

    export declare function ExecuteAction(request: ExecuteWorkflowActionRequest, onSuccess?: (response: Task) => void, opt?: ServiceOptions<any>): PromiseLike<Task>;
    export declare function GetPermittedActions(request: GetPermittedActionsRequest, onSuccess?: (response: GetPermittedActionsResponse) => void, opt?: ServiceOptions<any>): PromiseLike<GetPermittedActionsResponse>;

    export const Methods = {
        ExecuteAction: "Workflow/ExecuteAction",
        GetPermittedActions: "Workflow/GetPermittedActions"
    } as const;

    [
        'ExecuteAction', 
        'GetPermittedActions'
    ].forEach(x => {
        (<any>WorkflowService)[x] = function (r, s, o) {
            return serviceRequest(baseUrl + '/' + x, r, s, o);
        };
    });
}
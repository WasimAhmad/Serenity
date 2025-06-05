namespace Serene.Workflow {
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
            return Q.post(baseUrl + '/ExecuteAction', request);
        }

        export function GetPermittedActions(request: GetPermittedActionsRequest) {
            return Q.post<GetPermittedActionsResponse>(baseUrl + '/GetPermittedActions', request);
        }
    }
}

import { SaveRequest, SaveResponse, ServiceOptions, DeleteRequest, DeleteResponse, RetrieveRequest, RetrieveResponse, ListRequest, ListResponse, serviceRequest } from "@serenity-is/corelib";
import { WorkflowDefinitionRow } from "./WorkflowDefinitionRow";

export namespace WorkflowDefinitionService {
    export const baseUrl = 'Workflow/Definitions';

    export declare function Create(request: SaveRequest<WorkflowDefinitionRow>, onSuccess?: (response: SaveResponse) => void, opt?: ServiceOptions<any>): PromiseLike<SaveResponse>;
    export declare function Update(request: SaveRequest<WorkflowDefinitionRow>, onSuccess?: (response: SaveResponse) => void, opt?: ServiceOptions<any>): PromiseLike<SaveResponse>;
    export declare function Delete(request: DeleteRequest, onSuccess?: (response: DeleteResponse) => void, opt?: ServiceOptions<any>): PromiseLike<DeleteResponse>;
    export declare function Retrieve(request: RetrieveRequest, onSuccess?: (response: RetrieveResponse<WorkflowDefinitionRow>) => void, opt?: ServiceOptions<any>): PromiseLike<RetrieveResponse<WorkflowDefinitionRow>>;
    export declare function List(request: ListRequest, onSuccess?: (response: ListResponse<WorkflowDefinitionRow>) => void, opt?: ServiceOptions<any>): PromiseLike<ListResponse<WorkflowDefinitionRow>>;

    export const Methods = {
        Create: "Workflow/Definitions/Create",
        Update: "Workflow/Definitions/Update",
        Delete: "Workflow/Definitions/Delete",
        Retrieve: "Workflow/Definitions/Retrieve",
        List: "Workflow/Definitions/List"
    } as const;

    [
        'Create',
        'Update',
        'Delete',
        'Retrieve',
        'List'
    ].forEach(x => {
        (<any>WorkflowDefinitionService)[x] = function (r: any, s: any, o: any) {
            return serviceRequest(baseUrl + '/' + x, r, s, o);
        };
    });
}

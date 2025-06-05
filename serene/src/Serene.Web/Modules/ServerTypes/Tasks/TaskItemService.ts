import { SaveRequest, SaveResponse, ServiceOptions, DeleteRequest, DeleteResponse, RetrieveRequest, RetrieveResponse, ListRequest, ListResponse, serviceRequest } from "@serenity-is/corelib";
import { TaskItemRow } from "./TaskItemRow";

export namespace TaskItemService {
    export const baseUrl = 'Tasks/TaskItem';

    export declare function Create(request: SaveRequest<TaskItemRow>, onSuccess?: (response: SaveResponse) => void, opt?: ServiceOptions<any>): PromiseLike<SaveResponse>;
    export declare function Update(request: SaveRequest<TaskItemRow>, onSuccess?: (response: SaveResponse) => void, opt?: ServiceOptions<any>): PromiseLike<SaveResponse>;
    export declare function Delete(request: DeleteRequest, onSuccess?: (response: DeleteResponse) => void, opt?: ServiceOptions<any>): PromiseLike<DeleteResponse>;
    export declare function Retrieve(request: RetrieveRequest, onSuccess?: (response: RetrieveResponse<TaskItemRow>) => void, opt?: ServiceOptions<any>): PromiseLike<RetrieveResponse<TaskItemRow>>;
    export declare function List(request: ListRequest, onSuccess?: (response: ListResponse<TaskItemRow>) => void, opt?: ServiceOptions<any>): PromiseLike<ListResponse<TaskItemRow>>;

    export const Methods = {
        Create: "Tasks/TaskItem/Create",
        Update: "Tasks/TaskItem/Update",
        Delete: "Tasks/TaskItem/Delete",
        Retrieve: "Tasks/TaskItem/Retrieve",
        List: "Tasks/TaskItem/List"
    } as const;

    [
        'Create', 
        'Update', 
        'Delete', 
        'Retrieve', 
        'List'
    ].forEach(x => {
        (<any>TaskItemService)[x] = function (r, s, o) {
            return serviceRequest(baseUrl + '/' + x, r, s, o);
        };
    });
}
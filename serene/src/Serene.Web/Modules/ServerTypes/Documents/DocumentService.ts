import { SaveRequest, SaveResponse, ServiceOptions, DeleteRequest, DeleteResponse, RetrieveRequest, RetrieveResponse, ListRequest, ListResponse, serviceRequest } from "@serenity-is/corelib";
import { DocumentRow } from "./DocumentRow";

export namespace DocumentService {
    export const baseUrl = 'Documents/Document';

    export declare function Create(request: SaveRequest<DocumentRow>, onSuccess?: (response: SaveResponse) => void, opt?: ServiceOptions<any>): PromiseLike<SaveResponse>;
    export declare function Update(request: SaveRequest<DocumentRow>, onSuccess?: (response: SaveResponse) => void, opt?: ServiceOptions<any>): PromiseLike<SaveResponse>;
    export declare function Delete(request: DeleteRequest, onSuccess?: (response: DeleteResponse) => void, opt?: ServiceOptions<any>): PromiseLike<DeleteResponse>;
    export declare function Retrieve(request: RetrieveRequest, onSuccess?: (response: RetrieveResponse<DocumentRow>) => void, opt?: ServiceOptions<any>): PromiseLike<RetrieveResponse<DocumentRow>>;
    export declare function List(request: ListRequest, onSuccess?: (response: ListResponse<DocumentRow>) => void, opt?: ServiceOptions<any>): PromiseLike<ListResponse<DocumentRow>>;

    export const Methods = {
        Create: "Documents/Document/Create",
        Update: "Documents/Document/Update",
        Delete: "Documents/Document/Delete",
        Retrieve: "Documents/Document/Retrieve",
        List: "Documents/Document/List"
    } as const;

    [
        'Create', 
        'Update', 
        'Delete', 
        'Retrieve', 
        'List'
    ].forEach(x => {
        (<any>DocumentService)[x] = function (r, s, o) {
            return serviceRequest(baseUrl + '/' + x, r, s, o);
        };
    });
}

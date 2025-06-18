import { ServiceOptions, DeleteResponse, serviceRequest } from "@serenity-is/corelib";
import { WorkflowDefinitionDeleteRequest } from "./WorkflowDefinitionDeleteRequest";
import { WorkflowDefinitionListRequest } from "./WorkflowDefinitionListRequest";
import { WorkflowDefinitionListResponse } from "./WorkflowDefinitionListResponse";
import { WorkflowDefinitionRetrieveRequest } from "./WorkflowDefinitionRetrieveRequest";
import { WorkflowDefinitionRetrieveResponse } from "./WorkflowDefinitionRetrieveResponse";
import { WorkflowDefinitionSaveRequest } from "./WorkflowDefinitionSaveRequest";
import { WorkflowDefinitionSaveResponse } from "./WorkflowDefinitionSaveResponse";

export namespace WorkflowDefinitionManagementService {
    export const baseUrl = 'Workflow/WorkflowDefinitionManagement';

    export declare function Save(request: WorkflowDefinitionSaveRequest, onSuccess?: (response: WorkflowDefinitionSaveResponse) => void, opt?: ServiceOptions<any>): PromiseLike<WorkflowDefinitionSaveResponse>;
    export declare function Retrieve(request: WorkflowDefinitionRetrieveRequest, onSuccess?: (response: WorkflowDefinitionRetrieveResponse) => void, opt?: ServiceOptions<any>): PromiseLike<WorkflowDefinitionRetrieveResponse>;
    export declare function List(request: WorkflowDefinitionListRequest, onSuccess?: (response: WorkflowDefinitionListResponse) => void, opt?: ServiceOptions<any>): PromiseLike<WorkflowDefinitionListResponse>;
    export declare function Delete(request: WorkflowDefinitionDeleteRequest, onSuccess?: (response: DeleteResponse) => void, opt?: ServiceOptions<any>): PromiseLike<DeleteResponse>;

    export const Methods = {
        Save: "Workflow/WorkflowDefinitionManagement/Save",
        Retrieve: "Workflow/WorkflowDefinitionManagement/Retrieve",
        List: "Workflow/WorkflowDefinitionManagement/List",
        Delete: "Workflow/WorkflowDefinitionManagement/Delete"
    } as const;

    [
        'Save',
        'Retrieve',
        'List',
        'Delete'
    ].forEach(x => {
        (<any>WorkflowDefinitionManagementService)[x] = function (r, s, o) {
            return serviceRequest(baseUrl + '/' + x, r, s, o);
        };
    });
}
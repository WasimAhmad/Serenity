import { ServiceRequest } from "@serenity-is/corelib";
import { ApiPermissionGrantType } from "./ApiPermissionGrantType";

export interface ApiWorkflowTrigger extends ServiceRequest {
    Id?: string;
    TriggerKey?: string;
    DisplayName?: string;
    HandlerKey?: string;
    FormKey?: string;
    RequiresInput?: boolean;
    PermissionType?: ApiPermissionGrantType;
    Permissions?: string;
    PermissionHandlerKey?: string;
}
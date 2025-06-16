import { ServiceResponse } from "@serenity-is/corelib";

export interface GetPermittedActionsResponse extends ServiceResponse {
    Actions?: string[];
}
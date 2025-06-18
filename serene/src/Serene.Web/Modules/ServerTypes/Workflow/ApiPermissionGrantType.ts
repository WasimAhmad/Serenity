import { Decorators } from "@serenity-is/corelib";

export enum ApiPermissionGrantType {
    Explicit = 0,
    Hierarchy = 1,
    Handler = 2
}
Decorators.registerEnumType(ApiPermissionGrantType, 'Serene.Workflow.ApiPermissionGrantType');
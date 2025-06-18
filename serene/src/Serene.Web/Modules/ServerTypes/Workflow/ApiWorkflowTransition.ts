import { ServiceRequest } from "@serenity-is/corelib";

export interface ApiWorkflowTransition extends ServiceRequest {
    Id?: string;
    FromStateId?: string;
    ToStateId?: string;
    TriggerId?: string;
    GuardKey?: string;
}
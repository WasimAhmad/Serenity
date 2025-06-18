import { ServiceRequest } from "@serenity-is/corelib";

export interface ApiWorkflowState extends ServiceRequest {
    Id?: string;
    StateKey?: string;
    DisplayName?: string;
    X?: number;
    Y?: number;
}
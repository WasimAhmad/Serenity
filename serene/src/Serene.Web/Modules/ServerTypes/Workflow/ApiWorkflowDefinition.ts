import { ServiceRequest } from "@serenity-is/corelib";
import { ApiWorkflowState } from "./ApiWorkflowState";
import { ApiWorkflowTransition } from "./ApiWorkflowTransition";
import { ApiWorkflowTrigger } from "./ApiWorkflowTrigger";

export interface ApiWorkflowDefinition extends ServiceRequest {
    DefinitionId?: string;
    DefinitionName?: string;
    States?: ApiWorkflowState[];
    Triggers?: ApiWorkflowTrigger[];
    Transitions?: ApiWorkflowTransition[];
}
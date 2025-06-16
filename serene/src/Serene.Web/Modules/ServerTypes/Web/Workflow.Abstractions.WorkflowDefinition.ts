import { WorkflowState } from "./Workflow.Abstractions.WorkflowState";
import { WorkflowTransition } from "./Workflow.Abstractions.WorkflowTransition";
import { WorkflowTrigger } from "./Workflow.Abstractions.WorkflowTrigger";

export interface WorkflowDefinition {
    WorkflowKey?: string;
    InitialState?: string;
    States?: { [key: string]: WorkflowState };
    Triggers?: { [key: string]: WorkflowTrigger };
    Transitions?: WorkflowTransition[];
}
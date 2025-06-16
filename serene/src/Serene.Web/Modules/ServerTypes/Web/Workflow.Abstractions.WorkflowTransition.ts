export interface WorkflowTransition {
    From?: string;
    Trigger?: string;
    To?: string;
    GuardKey?: string;
}
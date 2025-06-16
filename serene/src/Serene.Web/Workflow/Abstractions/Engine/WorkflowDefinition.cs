using System.Collections.Generic;

namespace Serene.Web.Workflow.Abstractions
{
    public class WorkflowDefinition
    {
        public required string WorkflowKey { get; set; }
        public required string InitialState { get; set; }
        public Dictionary<string, WorkflowState> States { get; set; } = new();
        public Dictionary<string, WorkflowTrigger> Triggers { get; set; } = new();
        public List<WorkflowTransition> Transitions { get; set; } = new();
    }

    public class WorkflowState
    {
        public required string StateKey { get; set; }
        public string? DisplayName { get; set; }
    }

    public class WorkflowTrigger
    {
        public required string TriggerKey { get; set; }
        public string? DisplayName { get; set; }
        public string? HandlerKey { get; set; }
        public string? FormKey { get; set; }
        public bool RequiresInput { get; set; }
    }

    public class WorkflowTransition
    {
        public required string From { get; set; }
        public required string Trigger { get; set; }
        public required string To { get; set; }
        public string? GuardKey { get; set; }
    }
}

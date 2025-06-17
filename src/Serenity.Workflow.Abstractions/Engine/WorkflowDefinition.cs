using System.Collections.Generic;

namespace Serenity.Workflow
{
    /// <summary>
    /// Defines the mechanism used to grant permission for a workflow trigger.
    /// </summary>
    public enum PermissionGrantType
    {
        /// <summary>
        /// Permission is granted if the user's identifier (or their roles)
        /// is explicitly listed in the trigger's Permissions list.
        /// </summary>
        Explicit,
        ///// <summary>
        ///// Permission is determined by a hierarchical structure (e.g., organizational chart).
        ///// Requires custom logic or an IWorkflowPermissionHandler implementation.
        ///// </summary>
        //Hierarchy,
        /// <summary>
        /// Permission is determined by a registered IWorkflowPermissionHandler.
        /// The specific handler can be resolved via services, possibly using HandlerKey from the trigger.
        /// </summary>
        Handler
    }

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
        public PermissionGrantType PermissionType { get; set; }
        public List<string> Permissions { get; set; } = new();
        /// <summary>
        /// Optional key to resolve a custom <see cref="IWorkflowPermissionHandler"/>
        /// from the service provider, used when <see cref="PermissionType"/> is <see cref="PermissionGrantType.Handler"/>.
        /// If this is null when type is Handler, a default or globally registered permission handler might be used,
        /// or it might result in a configuration error if no suitable handler can be determined.
        /// </summary>
        public string? PermissionHandlerKey { get; set; }
    }

    public class WorkflowTransition
    {
        public required string From { get; set; }
        public required string Trigger { get; set; }
        public required string To { get; set; }
        public string? GuardKey { get; set; }
    }
}

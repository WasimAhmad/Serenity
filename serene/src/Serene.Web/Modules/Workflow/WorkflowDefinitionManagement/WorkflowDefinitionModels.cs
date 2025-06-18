using Serenity.Services;
using System.Collections.Generic;

namespace Serene.Workflow
{
    // Corresponds to PermissionGrantType in C# and TypeScript
    public enum ApiPermissionGrantType
    {
        Explicit = 0,
        Hierarchy = 1,
        Handler = 2
    }

    public class ApiWorkflowState : ServiceRequest
    {
        public string Id { get; set; } // Client-side ID, might be different from DB ID if new
        public string StateKey { get; set; }
        public string DisplayName { get; set; }
        // Potentially add X, Y coordinates for the designer UI
        public double? X { get; set; }
        public double? Y { get; set; }
    }

    public class ApiWorkflowTrigger : ServiceRequest
    {
        public string Id { get; set; } // Client-side ID
        public string TriggerKey { get; set; }
        public string DisplayName { get; set; }
        public string HandlerKey { get; set; } // For IWorkflowActionHandler
        public string FormKey { get; set; }
        public bool? RequiresInput { get; set; }
        public ApiPermissionGrantType? PermissionType { get; set; }
        public string Permissions { get; set; } // Comma-separated for Explicit
        public string PermissionHandlerKey { get; set; } // For IWorkflowPermissionHandler
    }

    public class ApiWorkflowTransition : ServiceRequest
    {
        public string Id { get; set; } // Client-side ID
        public string FromStateId { get; set; } // Refers to ApiWorkflowState.Id
        public string ToStateId { get; set; }   // Refers to ApiWorkflowState.Id
        public string TriggerId { get; set; }   // Refers to ApiWorkflowTrigger.Id
        public string GuardKey { get; set; } // Optional: Key for IWorkflowGuard
    }

    public class ApiWorkflowDefinition : ServiceRequest
    {
        public string DefinitionId { get; set; } // The main key for the workflow definition
        public string DefinitionName { get; set; } // A human-readable name
        public List<ApiWorkflowState> States { get; set; } = new List<ApiWorkflowState>();
        public List<ApiWorkflowTrigger> Triggers { get; set; } = new List<ApiWorkflowTrigger>();
        public List<ApiWorkflowTransition> Transitions { get; set; } = new List<ApiWorkflowTransition>();
    }

    public class WorkflowDefinitionSaveRequest : ServiceRequest
    {
        public ApiWorkflowDefinition Definition { get; set; }
    }

    public class WorkflowDefinitionSaveResponse : ServiceResponse { }

    public class WorkflowDefinitionRetrieveRequest : ServiceRequest
    {
        public string DefinitionId { get; set; }
    }

    public class WorkflowDefinitionRetrieveResponse : ServiceResponse
    {
        public ApiWorkflowDefinition Definition { get; set; }
    }

    public class WorkflowDefinitionListRequest : ListRequest { }

    public class WorkflowDefinitionListResponse : ListResponse<ApiWorkflowDefinition> { }

    public class WorkflowDefinitionDeleteRequest : DeleteRequest
    {
        public string DefinitionId { get; set; }
    }
    // DeleteResponse is already part of Serenity.Services

}

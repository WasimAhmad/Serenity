using Serenity.Workflow;

namespace Serene;

public class DocumentWorkflowDefinitionProvider : IWorkflowDefinitionProvider
{
    public WorkflowDefinition? GetDefinition(string workflowKey)
    {
        if (workflowKey != "DocumentWorkflow")
            return null;

        return new WorkflowDefinition
        {
            WorkflowKey = "DocumentWorkflow",
            InitialState = "Draft",
            States = new()
            {
                ["Draft"] = new WorkflowState { StateKey = "Draft", DisplayName = "Draft" },
                ["Submitted"] = new WorkflowState { StateKey = "Submitted", DisplayName = "Submitted" },
                ["UnderReview"] = new WorkflowState { StateKey = "UnderReview", DisplayName = "Under Review" },
                ["ChangesRequested"] = new WorkflowState { StateKey = "ChangesRequested", DisplayName = "Changes Requested" },
                ["Resubmitted"] = new WorkflowState { StateKey = "Resubmitted", DisplayName = "Resubmitted" },
                ["FinalReview"] = new WorkflowState { StateKey = "FinalReview", DisplayName = "Final Review" },
                ["Approved"] = new WorkflowState { StateKey = "Approved", DisplayName = "Approved" },
                ["Rejected"] = new WorkflowState { StateKey = "Rejected", DisplayName = "Rejected" }
            },
            Triggers = new()
            {
                ["Submit"] = new WorkflowTrigger { TriggerKey = "Submit", DisplayName = "Submit", RequiresInput = true },
                ["StartReview"] = new WorkflowTrigger { TriggerKey = "StartReview", DisplayName = "Start Review" },
                ["RequestChanges"] = new WorkflowTrigger { TriggerKey = "RequestChanges", DisplayName = "Request Changes", RequiresInput = true },
                ["Resubmit"] = new WorkflowTrigger { TriggerKey = "Resubmit", DisplayName = "Resubmit", RequiresInput = true },
                ["StartFinalReview"] = new WorkflowTrigger { TriggerKey = "StartFinalReview", DisplayName = "Start Final Review" },
                ["Approve"] = new WorkflowTrigger { TriggerKey = "Approve", DisplayName = "Approve" },
                ["Reject"] = new WorkflowTrigger { TriggerKey = "Reject", DisplayName = "Reject", RequiresInput = true }
            },
            Transitions = new()
            {
                new WorkflowTransition { From = "Draft", Trigger = "Submit", To = "Submitted" },
                new WorkflowTransition { From = "Submitted", Trigger = "StartReview", To = "UnderReview" },
                new WorkflowTransition { From = "UnderReview", Trigger = "RequestChanges", To = "ChangesRequested" },
                new WorkflowTransition { From = "ChangesRequested", Trigger = "Resubmit", To = "Resubmitted" },
                new WorkflowTransition { From = "Resubmitted", Trigger = "StartFinalReview", To = "FinalReview" },
                new WorkflowTransition { From = "UnderReview", Trigger = "Reject", To = "Rejected", GuardKey = typeof(Workflow.ApprovalPermissionGuard).FullName },
                new WorkflowTransition { From = "FinalReview", Trigger = "Approve", To = "Approved", GuardKey = typeof(Workflow.ApprovalPermissionGuard).FullName },
                new WorkflowTransition { From = "FinalReview", Trigger = "Reject", To = "Rejected", GuardKey = typeof(Workflow.ApprovalPermissionGuard).FullName }
            }
        };
    }
}

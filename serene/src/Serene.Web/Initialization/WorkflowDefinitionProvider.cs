using Serenity.Workflow;

namespace Serene;

public class WorkflowDefinitionProvider : IWorkflowDefinitionProvider
{
    private List<WorkflowDefinition> _workflowDefinitions = new();
    public WorkflowDefinitionProvider()
    {
        _workflowDefinitions.AddRange(
            new WorkflowDefinition
            {
                WorkflowKey = "TaskWorkflow",
                InitialState = "Open",
                States = new()
                {
                    ["Open"] = new WorkflowState { StateKey = "Open", DisplayName = "Open" },
                    ["InProgress"] = new WorkflowState { StateKey = "InProgress", DisplayName = "In Progress" },
                    ["Closed"] = new WorkflowState { StateKey = "Closed", DisplayName = "Closed" }
                },
                Triggers = new()
                {
                    ["Start"] = new WorkflowTrigger
                    {
                        TriggerKey = "Start",
                        DisplayName = "Start",
                        HandlerKey = typeof(Workflow.StartTaskWorkflowHandler).FullName
                    },
                    ["Finish"] = new WorkflowTrigger
                    {
                        TriggerKey = "Finish",
                        DisplayName = "Finish",
                        HandlerKey = typeof(Workflow.FinishTaskWorkflowHandler).FullName
                    }
                },
                Transitions = new()
            {
                new WorkflowTransition { From = "Open", Trigger = "Start", To = "InProgress" },
                new WorkflowTransition { From = "InProgress", Trigger = "Finish", To = "Closed" }
            }},
            new WorkflowDefinition
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
                    ["Submit"] = new WorkflowTrigger
                    {
                        TriggerKey = "Submit",
                        DisplayName = "Submit",
                        HandlerKey = typeof(Workflow.SubmitDocumentWorkflowHandler).FullName,
                        RequiresInput = true,
                        FormKey = "Workflow.DocumentSubmit"
                    },
                    ["StartReview"] = new WorkflowTrigger
                    {
                        TriggerKey = "StartReview",
                        DisplayName = "Start Review",
                        HandlerKey = typeof(Workflow.StartReviewDocumentWorkflowHandler).FullName
                    },
                    ["RequestChanges"] = new WorkflowTrigger
                    {
                        TriggerKey = "RequestChanges",
                        DisplayName = "Request Changes",
                        HandlerKey = typeof(Workflow.RequestChangesDocumentWorkflowHandler).FullName,
                        RequiresInput = true,
                        FormKey = "Workflow.DocumentRequestChanges"
                    },
                    ["Resubmit"] = new WorkflowTrigger
                    {
                        TriggerKey = "Resubmit",
                        DisplayName = "Resubmit",
                        HandlerKey = typeof(Workflow.ResubmitDocumentWorkflowHandler).FullName,
                        RequiresInput = true,
                        FormKey = "Workflow.DocumentResubmit"
                    },
                    ["StartFinalReview"] = new WorkflowTrigger
                    {
                        TriggerKey = "StartFinalReview",
                        DisplayName = "Start Final Review",
                        HandlerKey = typeof(Workflow.StartFinalReviewDocumentWorkflowHandler).FullName
                    },
                    ["Approve"] = new WorkflowTrigger
                    {
                        TriggerKey = "Approve",
                        DisplayName = "Approve",
                        HandlerKey = typeof(Workflow.ApproveDocumentWorkflowHandler).FullName
                    },
                    ["Reject"] = new WorkflowTrigger
                    {
                        TriggerKey = "Reject",
                        DisplayName = "Reject",
                        HandlerKey = typeof(Workflow.RejectDocumentWorkflowHandler).FullName,
                        RequiresInput = true,
                        FormKey = "Workflow.DocumentReject"
                    }
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
            }} );
    }
    public WorkflowDefinition? GetDefinition(string workflowKey)
    {
        if (workflowKey.IsNullOrEmpty())
            return null;

        return _workflowDefinitions.Find(x => x.WorkflowKey == workflowKey);
    }
}

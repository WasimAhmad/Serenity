using Serenity.Workflow;

namespace Serene;

public class SampleWorkflowDefinitionProvider : IWorkflowDefinitionProvider
{
    public WorkflowDefinition? GetDefinition(string workflowKey)
    {
        if (workflowKey != "TaskWorkflow")
            return null;

        return new WorkflowDefinition
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
                ["Start"] = new WorkflowTrigger { TriggerKey = "Start", DisplayName = "Start" },
                ["Finish"] = new WorkflowTrigger { TriggerKey = "Finish", DisplayName = "Finish" }
            },
            Transitions = new()
            {
                new WorkflowTransition { From = "Open", Trigger = "Start", To = "InProgress" },
                new WorkflowTransition { From = "InProgress", Trigger = "Finish", To = "Closed" }
            }
        };
    }
}

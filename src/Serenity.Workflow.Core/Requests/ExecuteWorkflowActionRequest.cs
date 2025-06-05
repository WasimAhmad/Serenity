namespace Serenity.Workflow
{
    public class ExecuteWorkflowActionRequest
    {
        public required string WorkflowKey { get; set; }
        public required string CurrentState { get; set; }
        public required string Trigger { get; set; }
        public IDictionary<string, object?>? Input { get; set; }
    }
}

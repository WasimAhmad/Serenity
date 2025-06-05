namespace Serenity.Workflow
{
    public class GetPermittedActionsRequest
    {
        public required string WorkflowKey { get; set; }
        public required string CurrentState { get; set; }
    }
}

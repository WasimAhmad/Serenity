using Serenity.Services;

namespace Serenity.Workflow;

public class GetWorkflowHistoryRequest : ServiceRequest
{
    public required string WorkflowKey { get; set; }
    public required object EntityId { get; set; }
}

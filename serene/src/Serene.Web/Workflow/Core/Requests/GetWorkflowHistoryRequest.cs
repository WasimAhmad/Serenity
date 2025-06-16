using Serenity.Services;

namespace Serene.Web.Workflow.Core;

public class GetWorkflowHistoryRequest : ServiceRequest
{
    public required string WorkflowKey { get; set; }
    public required object EntityId { get; set; }
}

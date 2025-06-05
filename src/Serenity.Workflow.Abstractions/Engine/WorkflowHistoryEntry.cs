using System;
namespace Serenity.Workflow;

public class WorkflowHistoryEntry
{
    public required string WorkflowKey { get; set; }
    public required object EntityId { get; set; }
    public required string FromState { get; set; }
    public required string ToState { get; set; }
    public required string Trigger { get; set; }
    public DateTime EventDate { get; set; } = DateTime.UtcNow;
    public string? User { get; set; }
}

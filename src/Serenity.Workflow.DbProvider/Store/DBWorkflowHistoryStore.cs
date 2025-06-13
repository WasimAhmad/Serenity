using Serenity.Data;
using System.Collections.Generic;
using System.Linq;

namespace Serenity.Workflow;

public class DBWorkflowHistoryStore(ISqlConnections connections) : IWorkflowHistoryStore
{
    private readonly ISqlConnections connections = connections ?? throw new ArgumentNullException(nameof(connections));

    public IEnumerable<WorkflowHistoryEntry> GetHistory(string workflowKey, object entityId)
    {
        using var connection = connections.NewByKey("Default");
        var fields = Entities.WorkflowHistoryRow.Fields;
        var list = connection.List<Entities.WorkflowHistoryRow>(
            fields.WorkflowKey == workflowKey & fields.EntityId == entityId.ToString());

        return list.Select(x => new WorkflowHistoryEntry
        {
            WorkflowKey = x.WorkflowKey!,
            EntityId = x.EntityId!,
            FromState = x.FromState!,
            ToState = x.ToState!,
            Trigger = x.Trigger!,
            Input = x.Input == null ? null : JSON.Parse<Dictionary<string, object?>>(x.Input),
            EventDate = x.EventDate ?? DateTime.UtcNow,
            User = x.User
        });
    }

    public void RecordEntry(WorkflowHistoryEntry entry)
    {
        using var connection = connections.NewByKey("Default");
        connection.Insert(new Entities.WorkflowHistoryRow
        {
            WorkflowKey = entry.WorkflowKey,
            EntityId = entry.EntityId.ToString()!,
            FromState = entry.FromState,
            ToState = entry.ToState,
            Trigger = entry.Trigger,
            Input = entry.Input == null ? null : JSON.Stringify(entry.Input, writeNulls: true),
            EventDate = entry.EventDate,
            User = entry.User
        });
    }
}

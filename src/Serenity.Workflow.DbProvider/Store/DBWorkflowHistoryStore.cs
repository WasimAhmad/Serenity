using Serenity.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Serenity.Workflow;

public class DBWorkflowHistoryStore(ISqlConnections connections, int batchSize = 1) : IWorkflowHistoryStore
{
    private readonly ISqlConnections connections = connections ?? throw new ArgumentNullException(nameof(connections));
    private readonly int batchSize = batchSize <= 0 ? 1 : batchSize;

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
            EventDate = x.EventDate != null ? DateTime.SpecifyKind(x.EventDate.Value, DateTimeKind.Utc) : DateTime.UtcNow,
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

    public async Task RecordEntryAsync(WorkflowHistoryEntry entry, CancellationToken cancellationToken = default)
    {
        await RecordEntriesAsync([entry], cancellationToken);
    }

    public async Task RecordEntriesAsync(IEnumerable<WorkflowHistoryEntry> entries, CancellationToken cancellationToken = default)
    {
        if (entries == null)
            return;

        var list = entries.ToList();
        if (list.Count == 0)
            return;

        await Task.Yield();

        using var connection = connections.NewByKey("Default");
        using var transaction = batchSize > 1 && list.Count > 1 ? connection.BeginTransaction() : null;

        foreach (var entry in list)
        {
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

        transaction?.Commit();
    }
}

using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Serenity.Workflow;

public class InMemoryWorkflowHistoryStore : IWorkflowHistoryStore
{
    private readonly ConcurrentDictionary<(string, object), List<WorkflowHistoryEntry>> store = new();

    public IEnumerable<WorkflowHistoryEntry> GetHistory(string workflowKey, object entityId)
    {
        if (store.TryGetValue((workflowKey, entityId), out var list))
            return list;
        return Enumerable.Empty<WorkflowHistoryEntry>();
    }

    public void RecordEntry(WorkflowHistoryEntry entry)
    {
        var list = store.GetOrAdd((entry.WorkflowKey, entry.EntityId), _ => new List<WorkflowHistoryEntry>());
        list.Add(entry);
    }
}

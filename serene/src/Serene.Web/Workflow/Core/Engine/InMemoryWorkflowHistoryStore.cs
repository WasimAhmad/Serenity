using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Serene.Web.Workflow.Abstractions;

namespace Serene.Web.Workflow.Core;

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

    public Task RecordEntriesAsync(IEnumerable<WorkflowHistoryEntry> entries, CancellationToken cancellationToken = default)
    {
        if (entries != null)
        {
            foreach (var entry in entries)
                RecordEntry(entry);
        }
        return Task.CompletedTask;
    }

    public Task RecordEntryAsync(WorkflowHistoryEntry entry, CancellationToken cancellationToken = default)
    {
        RecordEntry(entry);
        return Task.CompletedTask;
    }
}

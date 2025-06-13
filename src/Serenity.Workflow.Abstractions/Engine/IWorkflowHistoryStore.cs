using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
namespace Serenity.Workflow;

public interface IWorkflowHistoryStore
{
    void RecordEntry(WorkflowHistoryEntry entry);
    IEnumerable<WorkflowHistoryEntry> GetHistory(string workflowKey, object entityId);

    /// <summary>
    /// Records the entry asynchronously. Default implementation delegates to
    /// <see cref="RecordEntry"/>.
    /// </summary>
    /// <param name="entry">History entry</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>A task representing the async operation</returns>
    Task RecordEntryAsync(WorkflowHistoryEntry entry, CancellationToken cancellationToken = default)
    {
        RecordEntry(entry);
        return Task.CompletedTask;
    }

    /// <summary>
    /// Records multiple entries asynchronously. Default implementation calls
    /// <see cref="RecordEntry"/> for each entry.
    /// </summary>
    /// <param name="entries">Entries to record</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>A task representing the async operation</returns>
    Task RecordEntriesAsync(IEnumerable<WorkflowHistoryEntry> entries, CancellationToken cancellationToken = default)
    {
        foreach (var entry in entries)
            RecordEntry(entry);
        return Task.CompletedTask;
    }
}

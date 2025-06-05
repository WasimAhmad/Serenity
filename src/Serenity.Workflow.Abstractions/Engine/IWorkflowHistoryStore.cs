using System.Collections.Generic;
namespace Serenity.Workflow;

public interface IWorkflowHistoryStore
{
    void RecordEntry(WorkflowHistoryEntry entry);
    IEnumerable<WorkflowHistoryEntry> GetHistory(string workflowKey, object entityId);
}

using System.Collections.Generic;

namespace Serenity.Workflow;

public class WorkflowEngineOptions
{
    public bool UseInMemoryHistoryStore { get; set; }

    public IList<IWorkflowEventHandler> EventHandlers { get; } = new List<IWorkflowEventHandler>();
}

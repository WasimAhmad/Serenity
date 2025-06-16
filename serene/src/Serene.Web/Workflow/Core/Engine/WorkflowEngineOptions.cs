using System.Collections.Generic;

namespace Serene.Web.Workflow.Core;

public class WorkflowEngineOptions
{
    public bool UseInMemoryHistoryStore { get; set; }

    public IList<IWorkflowEventHandler> EventHandlers { get; } = new List<IWorkflowEventHandler>();
}

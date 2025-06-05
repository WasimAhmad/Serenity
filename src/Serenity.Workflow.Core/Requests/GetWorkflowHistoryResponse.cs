using Serenity.Services;
using System.Collections.Generic;

namespace Serenity.Workflow;

public class GetWorkflowHistoryResponse : ServiceResponse
{
    public IEnumerable<WorkflowHistoryEntry>? History { get; set; }
}

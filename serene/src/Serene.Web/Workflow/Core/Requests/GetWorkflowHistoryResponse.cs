using Serenity.Services;
using System.Collections.Generic;
using Serene.Web.Workflow.Abstractions;

namespace Serene.Web.Workflow.Core;

public class GetWorkflowHistoryResponse : ServiceResponse
{
    public IEnumerable<WorkflowHistoryEntry>? History { get; set; }
}

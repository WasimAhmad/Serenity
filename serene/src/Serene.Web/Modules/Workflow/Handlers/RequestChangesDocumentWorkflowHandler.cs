using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Serenity.Workflow;

namespace Serene.Workflow;

public class RequestChangesDocumentWorkflowHandler : IWorkflowActionHandler
{
    public Task ExecuteAsync(IServiceProvider services, object instance, IDictionary<string, object?>? input)
    {
        return Task.CompletedTask;
    }
}

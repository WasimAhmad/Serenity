using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Serenity.Workflow
{
    public interface ILongRunningWorkflowActionHandler : IWorkflowActionHandler
    {
        Task ExecuteAsync(IServiceProvider services, object instance, IDictionary<string, object?>? input, IProgress<double> progress, CancellationToken cancellationToken);
    }
}

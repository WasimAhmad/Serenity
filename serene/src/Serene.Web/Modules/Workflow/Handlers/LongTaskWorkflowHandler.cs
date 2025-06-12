using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Serenity.Workflow;

namespace Serene.Workflow;

public class LongTaskWorkflowHandler : ILongRunningWorkflowActionHandler
{
    public async Task ExecuteAsync(IServiceProvider services, object instance, IDictionary<string, object?>? input, IProgress<double> progress, CancellationToken cancellationToken)
    {
        for (int i = 1; i <= 5; i++)
        {
            cancellationToken.ThrowIfCancellationRequested();
            await Task.Delay(500, cancellationToken);
            progress.Report(i / 5.0);
        }
    }

    public Task ExecuteAsync(IServiceProvider services, object instance, IDictionary<string, object?>? input)
    {
        return ExecuteAsync(services, instance, input, new Progress<double>(_ => { }), CancellationToken.None);
    }
}

# Workflow Engine Extension

This repository includes a minimal workflow engine implementation. Workflows are defined in the database and loaded by `DatabaseWorkflowDefinitionProvider`. You can enable a row by using `[WorkflowEnabled("WorkflowKey")]` and specify the state field with `[WorkflowStateField]`.

Services are registered in `Startup.cs` via `AddSerenityWorkflow()` and `AddWorkflowDbProvider()`. Generic endpoints are available under `Services/Workflow`.

Database tables for the workflow entities are created automatically during application startup as the provider assembly now includes FluentMigrator migrations.

## Long Running Actions

Workflow actions might involve operations that take significant time to complete. Implement handlers using `ILongRunningWorkflowActionHandler` to report progress and support cancellation.

```
public class LongTaskWorkflowHandler : ILongRunningWorkflowActionHandler
{
    public async Task ExecuteAsync(IServiceProvider services, object instance,
        IDictionary<string, object?>? input, IProgress<double> progress,
        CancellationToken cancellationToken)
    {
        for (int i = 1; i <= 5; i++)
        {
            cancellationToken.ThrowIfCancellationRequested();
            await Task.Delay(500, cancellationToken);
            progress.Report(i / 5.0);
        }
    }
}
```

This handler reports incremental progress while executing and can be triggered through the `Process` action in the sample `TaskWorkflow`.

using System;
using Serenity.Data;
using Serene.Web.Workflow.Abstractions;

namespace Serene.Workflow;

public class FinishTaskWorkflowHandler(ISqlConnections connections) : IWorkflowActionHandler
{
    public Task ExecuteAsync(IServiceProvider services, object instance, IDictionary<string, object?>? input)
    {
        if (input is null || !input.TryGetValue("EntityId", out var id) || id is null)
            return Task.CompletedTask;

        var taskId = Convert.ToInt32(id);
        using var connection = connections.NewByKey("Default");
        var fields = Tasks.TaskItemRow.Fields;
        input.TryGetValue("CurrentState", out var cs);
        var update = new SqlUpdate(fields.TableName)
            .Set(fields.State, "Closed")
            .Where(fields.TaskId == taskId);
        if (cs is string state)
            update.Where(fields.State == state);
        var rows = update.Execute(connection, ExpectedRows.ZeroOrOne);
        if (rows == 0)
            throw new InvalidOperationException("Concurrent modification detected");

        return Task.CompletedTask;
    }
}

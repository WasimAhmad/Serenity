using System;
using Serenity.Data;
using Serenity.Workflow;

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
        new SqlUpdate(fields.TableName)
            .Set(fields.State, "Closed")
            .Where(fields.TaskId == taskId)
            .Execute(connection);

        return Task.CompletedTask;
    }
}

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Serenity.Data;
using Serenity.Workflow;

namespace Serene.Workflow;

public class RequestChangesDocumentWorkflowHandler(ISqlConnections connections) : IWorkflowActionHandler
{
    public Task ExecuteAsync(IServiceProvider services, object instance, IDictionary<string, object?>? input)
    {
        if (input is null || !input.TryGetValue("EntityId", out var id) || id is null)
            return Task.CompletedTask;

        var documentId = Convert.ToInt32(id);
        using var connection = connections.NewByKey("Default");
        var fields = Documents.DocumentRow.Fields;
        input.TryGetValue("CurrentState", out var cs);
        var update = new SqlUpdate(fields.TableName)
            .Set(fields.State, "ChangesRequested")
            .Where(fields.DocumentId == documentId);
        if (cs is string state)
            update.Where(fields.State == state);
        var rows = update.Execute(connection, ExpectedRows.ZeroOrOne);
        if (rows == 0)
            throw new InvalidOperationException("Concurrent modification detected");

        return Task.CompletedTask;
    }
}

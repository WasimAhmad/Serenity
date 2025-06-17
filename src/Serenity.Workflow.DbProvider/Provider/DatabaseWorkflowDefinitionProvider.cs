using Serenity.Data;
using Serenity.Workflow;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Serenity.Workflow.Provider
{
    public class DatabaseWorkflowDefinitionProvider : IWorkflowDefinitionProvider
    {
        private readonly ISqlConnections connections;

        public DatabaseWorkflowDefinitionProvider(ISqlConnections connections)
        {
            this.connections = connections;
        }

        public WorkflowDefinition? GetDefinition(string workflowKey)
        {
            using var connection = connections.NewByKey("Default");
            var fields = Entities.WorkflowDefinitionRow.Fields;
            var definition = connection.TryFirst<Entities.WorkflowDefinitionRow>(fields.WorkflowKey == workflowKey);
            if (definition == null)
                return null;
            var def = new WorkflowDefinition { WorkflowKey = workflowKey, InitialState = definition.InitialState! };
            def.States = connection.List<Entities.WorkflowStateRow>(Entities.WorkflowStateRow.Fields.DefinitionId == definition.Id!.Value)
                .ToDictionary(x => x.StateKey! , x => new WorkflowState { StateKey = x.StateKey!, DisplayName = x.Name });
            def.Triggers = connection.List<Entities.WorkflowTriggerRow>(Entities.WorkflowTriggerRow.Fields.DefinitionId == definition.Id!.Value)
                .ToDictionary(x => x.TriggerKey!, x => new WorkflowTrigger {
                    TriggerKey = x.TriggerKey!,
                    DisplayName = x.Name,
                    HandlerKey = x.HandlerKey,
                    RequiresInput = x.RequiresInput ?? false,
                    FormKey = x.FormKey,
                    PermissionType = (PermissionGrantType)(x.PermissionType ?? (int)PermissionGrantType.Explicit),
                    Permissions = string.IsNullOrEmpty(x.Permissions) ? new List<string>() : x.Permissions.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).ToList(),
                    PermissionHandlerKey = x.PermissionHandlerKey
                });
            def.Transitions = connection.List<Entities.WorkflowTransitionRow>(Entities.WorkflowTransitionRow.Fields.DefinitionId == definition.Id!.Value)
                .Select(x => new WorkflowTransition { From = x.FromState!, To = x.ToState!, Trigger = x.TriggerKey!, GuardKey = x.GuardKey }).ToList();
            return def;
        }
    }
}

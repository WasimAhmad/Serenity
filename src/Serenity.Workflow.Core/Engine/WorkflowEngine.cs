using System;
using System.Collections.Generic;

using Stateless;
using System.Threading.Tasks;

namespace Serenity.Workflow
{
    public class WorkflowEngine
    {
        private readonly IWorkflowDefinitionProvider definitionProvider;
        private readonly IServiceProvider services;
        private readonly IWorkflowHistoryStore historyStore;

        public WorkflowEngine(IWorkflowDefinitionProvider definitionProvider, IServiceProvider services)
        {
            this.definitionProvider = definitionProvider;
            this.services = services;
            this.historyStore = services.GetService<IWorkflowHistoryStore>() ?? new InMemoryWorkflowHistoryStore();
        }

        private StateMachine<string, string> CreateMachine(string workflowKey, string currentState)
        {
            ArgumentNullException.ThrowIfNull(workflowKey);
            ArgumentNullException.ThrowIfNull(currentState);

            var def = definitionProvider.GetDefinition(workflowKey) ??
                throw new InvalidOperationException($"Workflow {workflowKey} not found");

            var sm = new StateMachine<string, string>(currentState);
            foreach (var t in def.Transitions)
            {
                sm.Configure(t.From)
                    .PermitIf(t.Trigger, t.To,
                        () => CanExecuteGuardAsync(t.GuardKey).GetAwaiter().GetResult());
            }

            return sm;
        }

        private async Task<bool> CanExecuteGuardAsync(string? guardKey)
        {
            if (guardKey == null)
                return true;
            var guard = services.GetService(Type.GetType(guardKey)!) as IWorkflowGuard;
            if (guard == null)
                return false;
            return await guard.CanExecuteAsync(services, this);
        }

        public void ExecuteAsync(string workflowKey, string currentState, string trigger, IDictionary<string, object?>? input)
        {
            ArgumentNullException.ThrowIfNull(workflowKey);
            ArgumentNullException.ThrowIfNull(currentState);
            ArgumentNullException.ThrowIfNull(trigger);

            var machine = CreateMachine(workflowKey, currentState);
            WorkflowTrigger? action = null;
            definitionProvider.GetDefinition(workflowKey)?.Triggers.TryGetValue(trigger, out action);

            IWorkflowActionHandler? handler = null;
            if (action?.HandlerKey != null)
            {
                var handlerType = Type.GetType(action.HandlerKey);
                if (handlerType == null)
                    handlerType = AppDomain.CurrentDomain.GetAssemblies()
                        .Select(a => a.GetType(action.HandlerKey))
                        .FirstOrDefault(t => t != null);

                if (handlerType != null)
                    handler = services.GetService(handlerType) as IWorkflowActionHandler;
            }

            if (handler != null)
                handler.ExecuteAsync(services, this, input);

            var from = currentState;
            machine.Fire(trigger);
            var to = machine.State;
            object? entityId = null;
            if (input != null && input.TryGetValue("EntityId", out var val) && val != null)
                entityId = val;
            if (entityId != null)
            {
                historyStore.RecordEntry(new WorkflowHistoryEntry
                {
                    WorkflowKey = workflowKey,
                    EntityId = entityId,
                    FromState = from,
                    ToState = to,
                    Trigger = trigger
                });
            }
        }

        public IEnumerable<string> GetPermittedTriggers(string workflowKey, string state)
        {
            ArgumentNullException.ThrowIfNull(workflowKey);
            ArgumentNullException.ThrowIfNull(state);

            var machine = CreateMachine(workflowKey, state);
            machine.Activate();
            return machine.PermittedTriggers;
        }
    }
}

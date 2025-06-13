using Microsoft.Extensions.DependencyInjection;
using Stateless;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using Serenity.Abstractions;

namespace Serenity.Workflow
{
    public class WorkflowEngine
    {
        private readonly IWorkflowDefinitionProvider definitionProvider;
        private readonly IServiceProvider services;
        private readonly IWorkflowHistoryStore historyStore;
        private readonly WorkflowEngineOptions options;
        private readonly ConcurrentDictionary<string, Type?> guardTypeCache = new();
        private readonly ConcurrentDictionary<string, Type?> handlerTypeCache = new();

        public WorkflowEngine(IWorkflowDefinitionProvider definitionProvider,
            IServiceProvider services, IWorkflowHistoryStore historyStore,
            WorkflowEngineOptions options)
        {
            this.definitionProvider = definitionProvider;
            this.services = services;
            this.historyStore = historyStore ?? throw new ArgumentNullException(nameof(historyStore));
            this.options = options;
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

        private static Type? ResolveType(string key)
        {
            var type = Type.GetType(key);
            if (type == null)
                type = AppDomain.CurrentDomain.GetAssemblies()
                    .Select(a => a.GetType(key))
                    .FirstOrDefault(t => t != null);
            return type;
        }

        private async Task<bool> CanExecuteGuardAsync(string? guardKey)
        {
            if (string.IsNullOrEmpty(guardKey))
                return true;

            var guardType = guardTypeCache.GetOrAdd(guardKey!, ResolveType);
            if (guardType == null)
                return false;

            var guard = services.GetService(guardType) as IWorkflowGuard;
            if (guard == null)
                return false;

            return await guard.CanExecuteAsync(services, this);
        }

        public async Task ExecuteAsync(string workflowKey, string currentState, string trigger, IDictionary<string, object?>? input)
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
                var handlerType = handlerTypeCache.GetOrAdd(action.HandlerKey, ResolveType);
                if (handlerType != null)
                    handler = services.GetService(handlerType) as IWorkflowActionHandler;
            }

            if (handler != null)
                await handler.ExecuteAsync(services, this, input);

            foreach (var h in options.EventHandlers)
                await h.OnTriggerFiredAsync(services, this, workflowKey, currentState, trigger, input);

            var from = currentState;

            foreach (var h in options.EventHandlers)
                await h.OnExitStateAsync(services, this, workflowKey, from);

            await machine.FireAsync(trigger);

            var to = machine.State;

            foreach (var h in options.EventHandlers)
                await h.OnEnterStateAsync(services, this, workflowKey, to);
            object? entityId = null;
            if (input != null && input.TryGetValue("EntityId", out var val) && val != null)
                entityId = val;
            if (entityId != null)
            {
                IDictionary<string, object?>? historyInput = input == null ? null :
                    new Dictionary<string, object?>(input);
                historyInput?.Remove("EntityId");

                var userAccessor = services.GetService<IUserAccessor>();
                var userName = userAccessor?.User?.Identity?.Name;

                historyStore.RecordEntry(new WorkflowHistoryEntry
                {
                    WorkflowKey = workflowKey,
                    EntityId = entityId,
                    FromState = from,
                    ToState = to,
                    Trigger = trigger,
                    Input = historyInput,
                    User = string.IsNullOrEmpty(userName) ? null : userName
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

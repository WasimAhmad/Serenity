using Microsoft.Extensions.DependencyInjection;
using Serenity.Abstractions;
using Serenity.Services;
using Stateless;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Serenity.Workflow
{
    public class WorkflowEngine
    {
        private readonly IWorkflowDefinitionProvider definitionProvider;
        private readonly IServiceProvider services;
        private readonly IWorkflowHistoryStore historyStore;
        private readonly WorkflowEngineOptions options;
        private readonly IUserAccessor _userAccessor;
        private readonly ConcurrentDictionary<string, Type?> guardTypeCache = new();
        private readonly ConcurrentDictionary<string, Type?> handlerTypeCache = new();
        private static readonly ConcurrentDictionary<string, Type> resolvedTypes = new();
        private static readonly Type MissingTypeSentinel = typeof(void);

        public WorkflowEngine(IWorkflowDefinitionProvider definitionProvider,
            IServiceProvider services, IWorkflowHistoryStore historyStore,
            WorkflowEngineOptions options, IUserAccessor userAccessor)
        {
            this.definitionProvider = definitionProvider;
            this.services = services;
            this.historyStore = historyStore ?? throw new ArgumentNullException(nameof(historyStore));
            this.options = options;
            this._userAccessor = userAccessor ?? throw new ArgumentNullException(nameof(userAccessor));
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
            if (resolvedTypes.TryGetValue(key, out var cached))
            {
                if (cached == MissingTypeSentinel)
                    return null;

                return cached;
            }

            var type = Type.GetType(key);
            if (type == null)
            {
                foreach (var asm in AppDomain.CurrentDomain.GetAssemblies())
                {
                    type = asm.GetType(key);
                    if (type != null)
                        break;
                }
            }

            resolvedTypes[key] = type ?? MissingTypeSentinel;

            if (type == null)
                throw new InvalidOperationException($"Workflow type '{key}' could not be resolved. Ensure the assembly is referenced and the type name is correct.");

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

            var def = definitionProvider.GetDefinition(workflowKey) ??
                throw new InvalidOperationException($"Workflow {workflowKey} not found");

            if (!def.Triggers.ContainsKey(trigger))
                throw new InvalidOperationException($"Trigger '{trigger}' is not defined for workflow '{workflowKey}'");

            if (!def.States.ContainsKey(currentState))
                throw new InvalidOperationException($"State '{currentState}' is not defined for workflow '{workflowKey}'");

            var machine = CreateMachine(workflowKey, currentState);

            //if (input != null)
            //    input["CurrentState"] = currentState;

            if (!machine.CanFire(trigger))
                throw new InvalidOperationException($"Trigger '{trigger}' cannot be fired from state '{currentState}' for workflow '{workflowKey}'");

            // WorkflowTrigger? action = null;
            // def.Triggers.TryGetValue(trigger, out action);
            // Ensure action is not null, which it shouldn't be if CanFire passed and trigger is in def.Triggers
            if (!def.Triggers.TryGetValue(trigger, out var action) || action == null)
            {
                // This case should ideally be caught by earlier checks or CanFire logic
                throw new InvalidOperationException($"Trigger '{trigger}' definition not found or is invalid for workflow '{workflowKey}'.");
            }

            // Permission Check
            var currentUserIdentifier = _userAccessor.User?.GetIdentifier();
            if (string.IsNullOrEmpty(currentUserIdentifier))
            {
                throw new ValidationError("User not authenticated or identifier not available. Cannot execute workflow action.");
            }

            if (action.PermissionType == PermissionGrantType.Explicit)
            {
                if (action.Permissions == null || !action.Permissions.Contains(currentUserIdentifier))
                {
                    throw new ValidationError("Not authorized to execute this action.");
                }
            }
            else if (action.PermissionType == PermissionGrantType.Hierarchy)
            {
                // TODO: Implement hierarchical permission check
                throw new ValidationError("Hierarchical permissions are not yet implemented. Access denied.");
            }
            else if (action.PermissionType == PermissionGrantType.Handler)
            {
                if (string.IsNullOrEmpty(action.PermissionHandlerKey))
                {
                    // As per subtask, throw if key is missing.
                    // Alternatively, one might try to resolve a default handler if key is null/empty.
                    throw new ValidationError("Permission handler key is missing for handler-based permission.");
                }

                // Resolve the single IWorkflowPermissionHandler.
                // Users needing multiple distinct handlers should register a factory/broker handler
                // that internally dispatches based on PermissionHandlerKey or other trigger properties.
                var permissionHandler = services.GetService(typeof(IWorkflowPermissionHandler)) as IWorkflowPermissionHandler;
                if (permissionHandler == null)
                {
                    throw new ValidationError($"No IWorkflowPermissionHandler registered in DI. Cannot process handler-based permission for trigger '{action.TriggerKey}'.");
                }

                object? entity = null;
                input?.TryGetValue("Entity", out entity);

                // Ensure _userAccessor.User is not null before passing. The check for currentUserIdentifier handles this.
                // However, IsAuthorized expects ClaimsPrincipal which could be from a non-null _userAccessor.User.
                if (_userAccessor.User == null)
                {
                     // This should have been caught by the string.IsNullOrEmpty(currentUserIdentifier) check earlier,
                     // but as a safeguard before calling IsAuthorized:
                    throw new ValidationError("User not available for handler-based permission check.");
                }

                if (!permissionHandler.IsAuthorized(_userAccessor.User, action, entity, services))
                {
                    throw new ValidationError($"Not authorized by permission handler for trigger '{action.TriggerKey}'.");
                }
            }

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

                await historyStore.RecordEntryAsync(new WorkflowHistoryEntry
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

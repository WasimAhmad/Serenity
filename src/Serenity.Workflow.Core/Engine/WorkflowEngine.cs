using Stateless;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace Serenity.Workflow
{
    public class WorkflowEngine
    {
        private readonly IWorkflowDefinitionProvider definitionProvider;
        private readonly IServiceProvider services;
        private readonly ConcurrentDictionary<string, StateMachine<string, string>> machines = new();

        public WorkflowEngine(IWorkflowDefinitionProvider definitionProvider, IServiceProvider services)
        {
            this.definitionProvider = definitionProvider;
            this.services = services;
        }

        private StateMachine<string, string> GetMachine(string workflowKey)
        {
            return machines.GetOrAdd(workflowKey, key =>
            {
                var def = definitionProvider.GetDefinition(key) ?? throw new InvalidOperationException($"Workflow {key} not found");
                var sm = new StateMachine<string, string>(def.InitialState);
                foreach (var t in def.Transitions)
                {
                    sm.Configure(t.From)
                        .PermitIf(t.Trigger, t.To, () => CanExecuteGuardAsync(t.GuardKey).GetAwaiter().GetResult());
                }
                return sm;
            });
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

        public async Task ExecuteAsync(string workflowKey, string currentState, string trigger, IDictionary<string, object?>? input)
        {
            var machine = GetMachine(workflowKey);
            var action = definitionProvider.GetDefinition(workflowKey)?.Triggers[trigger];
            var handler = action?.HandlerKey != null ? services.GetService(Type.GetType(action.HandlerKey)!) as IWorkflowActionHandler : null;
            if (handler != null)
                await handler.ExecuteAsync(services, this, input);
            machine.Fire(trigger);
        }

        public IEnumerable<string> GetPermittedTriggers(string workflowKey, string state)
        {
            var machine = GetMachine(workflowKey);
            machine.Activate();
            return machine.PermittedTriggers;
        }
    }
}

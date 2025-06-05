using Stateless;
using System.Threading.Tasks;

namespace Serenity.Workflow
{
    public class WorkflowEngine
    {
        private readonly IWorkflowDefinitionProvider definitionProvider;
        private readonly IServiceProvider services;

        public WorkflowEngine(IWorkflowDefinitionProvider definitionProvider, IServiceProvider services)
        {
            this.definitionProvider = definitionProvider;
            this.services = services;
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

        public async Task ExecuteAsync(string workflowKey, string currentState, string trigger, IDictionary<string, object?>? input)
        {
            ArgumentNullException.ThrowIfNull(workflowKey);
            ArgumentNullException.ThrowIfNull(currentState);
            ArgumentNullException.ThrowIfNull(trigger);

            var machine = CreateMachine(workflowKey, currentState);
            var action = definitionProvider.GetDefinition(workflowKey)?.Triggers[trigger];
            var handler = action?.HandlerKey != null ? services.GetService(Type.GetType(action.HandlerKey)!) as IWorkflowActionHandler : null;
            if (handler != null)
                await handler.ExecuteAsync(services, this, input);
            machine.Fire(trigger);
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

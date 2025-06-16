using Microsoft.Extensions.DependencyInjection;
using Serene.Web.Workflow.Abstractions;
using Serene.Web.Workflow.Core;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace Serene.Tests.Workflow
{
    public class WorkflowEngineTests
    {
        private class SimpleProvider : IWorkflowDefinitionProvider
        {
            public WorkflowDefinition? GetDefinition(string workflowKey)
            {
                return new WorkflowDefinition
                {
                    WorkflowKey = "Test",
                    InitialState = "Draft",
                    States = new()
                    {
                        ["Draft"] = new WorkflowState { StateKey = "Draft" },
                        ["Submitted"] = new WorkflowState { StateKey = "Submitted" }
                    },
                    Triggers = new()
                    {
                        ["Submit"] = new WorkflowTrigger { TriggerKey = "Submit" }
                    },
                    Transitions = new()
                    {
                        new WorkflowTransition { From = "Draft", To = "Submitted", Trigger = "Submit" }
                    }
                };
            }
        }

        private class TestHandler : IWorkflowEventHandler
        {
            public List<string> EnteredStates { get; } = new();
            public List<string> ExitedStates { get; } = new();
            public List<string> FiredTriggers { get; } = new();

            public Task OnTriggerFiredAsync(IServiceProvider services, WorkflowEngine engine,
                string workflowKey, string fromState, string trigger,
                IDictionary<string, object?>? input)
            {
                FiredTriggers.Add(trigger);
                return Task.CompletedTask;
            }

            public Task OnExitStateAsync(IServiceProvider services, WorkflowEngine engine,
                string workflowKey, string state)
            {
                ExitedStates.Add(state);
                return Task.CompletedTask;
            }

            public Task OnEnterStateAsync(IServiceProvider services, WorkflowEngine engine,
                string workflowKey, string state)
            {
                EnteredStates.Add(state);
                return Task.CompletedTask;
            }
        }

        [Fact]
        public async Task CanFireTrigger()
        {
            var services = new ServiceCollection();
            services.AddSingleton<IWorkflowDefinitionProvider, SimpleProvider>();
            services.AddSerenityWorkflow(o => o.UseInMemoryHistoryStore = true);
            var provider = services.BuildServiceProvider();
            var engine = provider.GetRequiredService<WorkflowEngine>();
            await engine.ExecuteAsync("Test", "Draft", "Submit", null);
            var permitted = engine.GetPermittedTriggers("Test", "Submitted");
            Assert.DoesNotContain("Submit", permitted);
        }

        [Fact]
        public void GetPermittedTriggersThrowsOnNullWorkflowKey()
        {
            var services = new ServiceCollection();
            services.AddSingleton<IWorkflowDefinitionProvider, SimpleProvider>();
            services.AddSerenityWorkflow(o => o.UseInMemoryHistoryStore = true);
            var provider = services.BuildServiceProvider();
            var engine = provider.GetRequiredService<WorkflowEngine>();

            Assert.Throws<ArgumentNullException>(() => engine.GetPermittedTriggers(null!, "Draft"));
        }

        [Fact]

        public async Task EventsAreFired()
        {
            var handler = new TestHandler();
            var services = new ServiceCollection();
            services.AddSingleton<IWorkflowDefinitionProvider, SimpleProvider>();
            services.AddSerenityWorkflow(o =>
            {
                o.UseInMemoryHistoryStore = true;
                o.EventHandlers.Add(handler);
            });
            var provider = services.BuildServiceProvider();
            var engine = provider.GetRequiredService<WorkflowEngine>();
            await engine.ExecuteAsync("Test", "Draft", "Submit", null);
            Assert.Contains("Submit", handler.FiredTriggers);
            Assert.Contains("Draft", handler.ExitedStates);
            Assert.Contains("Submitted", handler.EnteredStates);
        }

        public async Task ExecuteAsyncThrowsOnUnknownTrigger()
        {
            var services = new ServiceCollection();
            services.AddSingleton<IWorkflowDefinitionProvider, SimpleProvider>();
            services.AddSerenityWorkflow(o => o.UseInMemoryHistoryStore = true);
            var provider = services.BuildServiceProvider();
            var engine = provider.GetRequiredService<WorkflowEngine>();

            await Assert.ThrowsAsync<InvalidOperationException>(() => engine.ExecuteAsync("Test", "Draft", "Unknown", null));
        }

        [Fact]
        public async Task ExecuteAsyncThrowsWhenTriggerNotAllowedFromState()
        {
            var services = new ServiceCollection();
            services.AddSingleton<IWorkflowDefinitionProvider, SimpleProvider>();
            services.AddSerenityWorkflow(o => o.UseInMemoryHistoryStore = true);
            var provider = services.BuildServiceProvider();
            var engine = provider.GetRequiredService<WorkflowEngine>();

            await Assert.ThrowsAsync<InvalidOperationException>(() => engine.ExecuteAsync("Test", "Submitted", "Submit", null));

        }
    }
}

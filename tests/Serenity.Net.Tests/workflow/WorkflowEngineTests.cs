using Microsoft.Extensions.DependencyInjection;
using Serenity.Workflow;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace Serenity.Net.Tests.Workflow
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

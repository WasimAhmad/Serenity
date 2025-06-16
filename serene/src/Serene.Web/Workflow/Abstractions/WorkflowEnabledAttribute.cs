using System;

namespace Serene.Web.Workflow.Abstractions
{
    [AttributeUsage(AttributeTargets.Class)]
    public class WorkflowEnabledAttribute : Attribute
    {
        public WorkflowEnabledAttribute(string workflowKey)
        {
            WorkflowKey = workflowKey;
        }

        public string WorkflowKey { get; }
    }
}

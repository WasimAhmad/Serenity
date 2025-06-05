using System;

namespace Serenity.Workflow
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

using Serenity.Abstractions;
using Serene.Web.Workflow.Abstractions;

namespace Serene.Workflow;

public class ApprovalPermissionGuard(IPermissionService permissions) : IWorkflowGuard
{
    public Task<bool> CanExecuteAsync(IServiceProvider services, object instance)
    {
        return Task.FromResult(permissions.HasPermission("Document:Approve"));
    }
}

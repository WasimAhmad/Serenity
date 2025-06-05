using Serenity.Abstractions;
using Serenity.Workflow;

namespace Serene.Workflow;

public class ApprovalPermissionGuard(IPermissionService permissions) : IWorkflowGuard
{
    public Task<bool> CanExecuteAsync(IServiceProvider services, object instance)
    {
        return Task.FromResult(permissions.HasPermission("Document:Approve"));
    }
}

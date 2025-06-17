using System;
using System.Security.Claims;

namespace Serenity.Workflow
{
    /// <summary>
    /// Interface for custom workflow permission handlers, used when
    /// PermissionType on a trigger is set to Hierarchy or another custom type.
    /// </summary>
    public interface IWorkflowPermissionHandler
    {
        /// <summary>
        /// Checks if the specified user is authorized to execute the given trigger,
        /// potentially based on hierarchical rules or other custom logic.
        /// </summary>
        /// <param name="user">The current user.</param>
        /// <param name="trigger">The workflow trigger definition.</param>
        /// <param name="entity">The entity instance involved in the workflow (can be null).</param>
        /// <param name="services">Service provider for resolving dependencies.</param>
        /// <returns>True if the user is authorized, otherwise false.</returns>
        bool IsAuthorized(ClaimsPrincipal user, WorkflowTrigger trigger, object entity, IServiceProvider services);
    }
}

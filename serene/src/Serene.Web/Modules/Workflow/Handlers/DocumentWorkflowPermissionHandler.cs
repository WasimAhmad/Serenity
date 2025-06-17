using Serenity.Workflow;
using Serene.Documents;
using System;
using System.Security.Claims;
// For IsInRole, ClaimsPrincipal extensions are usually in System.Security.Claims
// For IPermissionService or Authorization related utilities, if needed:
// using Serenity.Abstractions; or using Serenity.Authorization;

namespace Serene.Workflow
{
    /// <summary>
    /// Custom workflow permission handler specific to Document entities.
    /// Determines authorization based on the DocumentType and user roles.
    /// </summary>
    public class DocumentWorkflowPermissionHandler : IWorkflowPermissionHandler
    {
        /// <summary>
        /// Checks if the specified user is authorized to execute a trigger
        /// related to a Document entity.
        /// </summary>
        /// <param name="user">The current user.</param>
        /// <param name="trigger">The workflow trigger definition. This might be used
        /// if different triggers have different rules, though not used in this specific implementation.</param>
        /// <param name="entity">The Document entity instance. Must be a DocumentRow.</param>
        /// <param name="services">Service provider. Not used in this specific implementation.</param>
        /// <returns>True if the user is authorized, otherwise false.</returns>
        public bool IsAuthorized(ClaimsPrincipal user, WorkflowTrigger trigger, object entity, IServiceProvider services)
        {
            if (user == null)
                return false;

            if (entity is not DocumentRow document)
            {
                // This handler is only for DocumentRow entities
                return false;
            }

            // The PermissionHandlerKey from the trigger could be used here if different document-related
            // permissions need different handling by this one class. For example, trigger.PermissionHandlerKey == "ReviewPermission".
            // For now, we'll assume this handler is specifically assigned to triggers where this logic applies.

            switch (document.DocumentType)
            {
                case DocumentType.Public:
                    // All authenticated users can access public documents
                    return true;

                case DocumentType.Internal:
                    // Only users in the "InternalDocs" role can access internal documents
                    return user.IsInRole("InternalDocs");

                // Potentially more cases for other DocumentTypes like "Confidential", "Restricted" etc.
                // case DocumentType.Confidential:
                //     return user.IsInRole("ConfidentialDocs") || user.HasPermission("Document:ViewConfidential");

                default:
                    // Deny by default for unknown or other document types
                    return false;
            }
        }
    }
}

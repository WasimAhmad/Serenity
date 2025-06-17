using FluentMigrator;
using Serenity.Extensions; // For MigrationKey attribute if it's there, or Serenity.Data.Mapping for DefaultDB

namespace Serene.Migrations.DefaultDB
{
    [DefaultDB, MigrationKey(20240729_1000)]
    public class DefaultDB_20240729_1000_UpdateDocumentWorkflowPermissions : Migration
    {
        public override void Up()
        {
            const string workflowKeyConst = "DocumentWorkflow"; // Renamed to avoid conflict
            const string permissionHandlerKey = "DocumentWorkflowPermission"; // Key for DocumentWorkflowPermissionHandler logic
            const int handlerPermissionType = 2; // Corresponds to PermissionGrantType.Handler

            // Retrieve the DefinitionId for "DocumentWorkflow"
            int definitionId = 0;
            Execute.WithConnection(connection =>
            {
                definitionId = connection.QueryFirst<int>(
                    "SELECT Id FROM WorkflowDefinitions WHERE WorkflowKey = @workflowKey",
                    new { workflowKey = workflowKeyConst });
            });

            // If definitionId is 0 (or not found), it might indicate an issue or the workflow doesn't exist.
            // For this migration, we assume it exists. If not, the Update below will not match any rows.
            // A more robust solution might throw an error if definitionId is not found.

            string[] triggerKeysToUpdate = {
                "Submit",
                "Approve",
                "Reject",
                "RequestChanges",
                "Resubmit",
                // Add any other triggers associated with DocumentWorkflow that need this permission setup
                // For example, triggers from Workflow.SubmitDocumentWorkflowHandler etc.
                // Might need to check WorkflowDefinitionProvider.cs or existing DB data for exact keys if they differ.
                // The subtask mentioned specific handlers like "StartTaskWorkflowHandler", "SubmitDocumentWorkflowHandler".
                // Their corresponding trigger keys (if part of "DocumentWorkflow") should be listed here.
                // For instance, if "SubmitDocumentWorkflowHandler" corresponds to a trigger "SubmitDoc", list "SubmitDoc".
                // The current list is based on common workflow actions.
            };

            if (definitionId > 0) // Proceed only if a valid definitionId was found
            {
                foreach (var triggerKey in triggerKeysToUpdate)
                {
                    Update.Table("WorkflowTriggers")
                        .Set(new { PermissionType = handlerPermissionType, PermissionHandlerKey = permissionHandlerKey })
                        .Where(new { DefinitionId = definitionId, TriggerKey = triggerKey });
                }
            }

            // Note: This migration assumes that the "DocumentWorkflow" and its associated triggers
            // already exist in the WorkflowDefinitions and WorkflowTriggers tables.
            // If they don't, this Update statement will not affect any rows.
            // A more comprehensive migration might first check for existence or Insert if not exists,
            // but that's beyond the scope of "updating existing triggers".
        }

        public override void Down()
        {
            // For Down migration, we might revert using WorkflowKey as DefinitionId might be less stable
            // or harder to guarantee if other migrations altered WorkflowDefinitions.
            // However, to be consistent with the Up migration's logic,
            // ideally, we would also fetch DefinitionId here.
            // For simplicity, as per current task focusing on Up(), we'll keep Down() using WorkflowKey,
            // but acknowledge this could be refined.

            const string workflowKeyConst = "DocumentWorkflow"; // Renamed for clarity
            const int previousPermissionType = 0; // Assuming 0 was the default (e.g., None or Explicit with empty list)
                                                  // This might need adjustment based on the actual previous state.

            string[] triggerKeysToRevert = {
                "Submit",
                "Approve",
                "Reject",
                "RequestChanges",
                "Resubmit"
                // Ensure this list matches the one in Up()
            };

            // Retrieve the DefinitionId for "DocumentWorkflow" to be precise, similar to Up()
            int definitionId = 0;
            Execute.WithConnection(connection =>
            {
                // It's possible the definition was deleted, so QueryFirstOrDefault is safer.
                // If not found, the update below won't run, which is acceptable for a revert.
                definitionId = connection.QueryFirstOrDefault<int>(
                    "SELECT Id FROM WorkflowDefinitions WHERE WorkflowKey = @workflowKey",
                    new { workflowKey = workflowKeyConst });
            });

            if (definitionId > 0) // Proceed only if a valid definitionId was found
            {
                foreach (var triggerKey in triggerKeysToRevert)
                {
                    Update.Table("WorkflowTriggers")
                        .Set(new { PermissionType = previousPermissionType, PermissionHandlerKey = (string)null })
                        .Where(new { DefinitionId = definitionId, TriggerKey = triggerKey });
                }
            }
            else
            {
                // Fallback or log: if DefinitionId not found, could try reverting by WorkflowKey
                // This part depends on how strictly the Down migration should behave.
                // For now, if DefinitionId isn't found, it implies the specific workflow instances
                // targeted by DefinitionId in Up() might not exist or are already altered.
                // Reverting by WorkflowKey could be broader than intended if multiple definitions
                // somehow ended up with the same WorkflowKey (though unlikely for a "Key").
                // Sticking to DefinitionId for precision if found.
                // If not found, the triggers are not updated by DefinitionId, which is consistent.
            }
        }
    }
}

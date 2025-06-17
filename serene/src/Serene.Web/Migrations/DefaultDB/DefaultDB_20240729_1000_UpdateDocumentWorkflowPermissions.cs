using FluentMigrator;
using Serenity.Extensions; // For MigrationKey attribute if it's there, or Serenity.Data.Mapping for DefaultDB

namespace Serene.Migrations.DefaultDB
{
    [DefaultDB, MigrationKey(20240729_1000)]
    public class DefaultDB_20240729_1000_UpdateDocumentWorkflowPermissions : Migration
    {
        public override void Up()
        {
            const string workflowKey = "DocumentWorkflow";
            const string permissionHandlerKey = "DocumentWorkflowPermission"; // Key for DocumentWorkflowPermissionHandler logic
            const int handlerPermissionType = 2; // Corresponds to PermissionGrantType.Handler

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

            foreach (var triggerKey in triggerKeysToUpdate)
            {
                Update.Table("WorkflowTriggers")
                    .Set(new { PermissionType = handlerPermissionType, PermissionHandlerKey = permissionHandlerKey })
                    .Where(new { WorkflowKey = workflowKey, TriggerKey = triggerKey });
            }

            // Note: This migration assumes that the "DocumentWorkflow" and its associated triggers
            // already exist in the WorkflowDefinitions and WorkflowTriggers tables.
            // If they don't, this Update statement will not affect any rows.
            // A more comprehensive migration might first check for existence or Insert if not exists,
            // but that's beyond the scope of "updating existing triggers".
        }

        public override void Down()
        {
            const string workflowKey = "DocumentWorkflow";
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

            foreach (var triggerKey in triggerKeysToRevert)
            {
                Update.Table("WorkflowTriggers")
                    .Set(new { PermissionType = previousPermissionType, PermissionHandlerKey = (string)null })
                    .Where(new { WorkflowKey = workflowKey, TriggerKey = triggerKey });
            }
        }
    }
}

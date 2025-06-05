# Workflow Engine Extension

This repository includes a minimal workflow engine implementation. Workflows are defined in the database and loaded by `DatabaseWorkflowDefinitionProvider`. You can enable a row by using `[WorkflowEnabled("WorkflowKey")]` and specify the state field with `[WorkflowStateField]`.

Services are registered in `Startup.cs` via `AddSerenityWorkflow()` and `AddWorkflowDbProvider()`. Generic endpoints are available under `Services/Workflow`.

Database tables for the workflow entities are created automatically during application startup as the provider assembly now includes FluentMigrator migrations.

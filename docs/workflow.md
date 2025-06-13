# Workflow Engine Extension

This repository includes a minimal workflow engine implementation. Workflows are defined in the database and loaded by `DatabaseWorkflowDefinitionProvider`. You can enable a row by using `[WorkflowEnabled("WorkflowKey")]` and specify the state field with `[WorkflowStateField]`.

Services are registered in `Startup.cs` via `AddSerenityWorkflow()` and `AddWorkflowDbProvider()`. The workflow engine no longer falls back to an in-memory history store. Use `AddSerenityWorkflow(o => o.UseInMemoryHistoryStore = true)` to enable the in-memory store or register another `IWorkflowHistoryStore` like the one added by `AddWorkflowDbProvider()`. Generic endpoints are available under `Services/Workflow`.

History store implementations can now record entries asynchronously. `IWorkflowHistoryStore` includes `RecordEntryAsync` and `RecordEntriesAsync` methods which may optionally batch writes when using database-backed stores.

Database tables for the workflow entities are created automatically during application startup as the provider assembly now includes FluentMigrator migrations.

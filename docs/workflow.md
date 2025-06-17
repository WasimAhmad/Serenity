# Workflow Engine Extension

This repository includes a minimal workflow engine implementation. Workflows are defined, typically in the database, and loaded by a `IWorkflowDefinitionProvider` implementation, such as `DatabaseWorkflowDefinitionProvider`.

You can integrate workflow capabilities with your entities by using the `[WorkflowEnabled("WorkflowKey")]` attribute on a row and specifying the state field with `[WorkflowStateField]`.

## Setup

Services for the workflow engine are registered in your application's startup configuration (e.g., `Startup.cs` or `Program.cs`) via extension methods:
- `AddSerenityWorkflow()`: Registers core workflow services.
- `AddWorkflowDbProvider()`: Registers the database-backed provider for workflow definitions and history.

## Workflow Definitions

A workflow is defined by several components: states, triggers that cause transitions between states, and the transitions themselves. These are represented by the `WorkflowDefinition` class, which aggregates information about states, triggers, and transitions.

### States

A workflow state (`WorkflowState`) represents a specific status or stage in your workflow process.
- `StateKey` (string, required): A unique key for the state (e.g., "Draft", "AwaitingApproval", "Published").
- `DisplayName` (string, optional): A human-readable name for the state.

### Triggers

A workflow trigger (`WorkflowTrigger`) represents an action or event that can cause a transition from one state to another.
- `TriggerKey` (string, required): A unique key for the trigger (e.g., "Submit", "Approve", "Reject").
- `DisplayName` (string, optional): A human-readable name for the trigger.
- `HandlerKey` (string, optional): Key to resolve an `IWorkflowActionHandler` for custom logic when the trigger is fired.
- `FormKey` (string, optional): Key for a form to be displayed for user input when this trigger is activated.
- `RequiresInput` (bool): Indicates if the trigger requires input (often used with `FormKey`).

### Trigger Permissions

The workflow engine allows defining permissions for triggers to control who can execute specific actions. This is configured using the `PermissionType` and `Permissions` properties on a `WorkflowTrigger`.

-   **`PermissionType` Property**:
    This property is an enum of type `PermissionGrantType` that determines how permission checks are performed. It has the following values:
    -   `Explicit`: Permissions are granted only if the current user's identifier (or a role they belong to) is explicitly listed in the `Permissions` property of the trigger.
    -   `Hierarchy`: Permissions are determined based on a hierarchical relationship (e.g., organizational structure, document ownership). *This logic is currently a placeholder in the `WorkflowEngine` and would require custom implementation by the end-user or will be addressed in a future update. The engine currently denies access if this type is set and no custom logic is implemented.*
    -   `Handler`: Permission is delegated to a custom implementation of `IWorkflowPermissionHandler` registered in your application's service container. This allows for complex, dynamic permission logic.

-   **`Permissions` Property**:
    This is a `List<string>` that holds identifiers used for `Explicit` permission checks. These identifiers can be:
    -   Usernames or user IDs (e.g., "john.doe", "12345").
    -   Role names (e.g., "role:Administrators", "role:Editors"). The prefix "role:" is a convention and actual role checking logic might depend on your `IUserAccessor` implementation.
    -   Other custom keys relevant to your application's permission system.

-   **Configuring Explicit Permissions**:
    To use explicit permissions, set the `PermissionType` to `Explicit` and populate the `Permissions` list with the identifiers of users/roles that are allowed to fire the trigger. For example, if only "user1" and members of "role:Approvers" can execute a trigger, the `Permissions` list would contain `["user1", "role:Approvers"]`.

-   **Hierarchical Permissions Status**:
    The `Hierarchy` permission type is designed for more complex scenarios where permissions derive from relationships not captured by simple lists. As mentioned, the core `WorkflowEngine` contains a `// TODO: Implement hierarchical permission check` comment where this logic should be added. If you intend to use this, you will need to modify the `WorkflowEngine` or provide a custom mechanism to evaluate these permissions. By default, if `PermissionType` is `Hierarchy`, the engine will currently deny the action.

-   **Using `PermissionGrantType.Handler` with `IWorkflowPermissionHandler`**:
    When `PermissionType` is set to `Handler`, the `WorkflowEngine` defers the authorization decision to a custom permission handler that you implement and register.
    -   **Implementation**: Create a class that implements the `Serenity.Workflow.IWorkflowPermissionHandler` interface. This interface has one method:
        ```csharp
        bool IsAuthorized(System.Security.Claims.ClaimsPrincipal user,
                          WorkflowTrigger trigger,
                          object entity,
                          System.IServiceProvider services);
        ```
        The `trigger` argument provides access to the `PermissionHandlerKey` (and other trigger properties), `entity` is the workflow target entity (if available, otherwise null), and `services` can be used to resolve further dependencies.
    -   **Registration**: Register your custom handler in your application's dependency injection container (e.g., in `Startup.cs` or `Program.cs`):
        ```csharp
        services.AddSingleton<IWorkflowPermissionHandler, MyCustomPermissionHandler>();
        // Or services.AddTransient, services.AddScoped as appropriate.
        ```
    -   **`PermissionHandlerKey`**: The `WorkflowTrigger` has a `PermissionHandlerKey` (string) property.
        -   The `WorkflowEngine` checks if this key is provided when `PermissionType` is `Handler`. If it's null or empty, an error will occur.
        -   Your `IWorkflowPermissionHandler` implementation can use this key (available via `trigger.PermissionHandlerKey` in the `IsAuthorized` method) to manage different sets of permission logic or to identify the specific rule to apply. For instance, if you have one central handler managing multiple types of dynamic permissions, this key can act as a discriminator.
    -   **Broker/Factory Pattern**: If you need several distinct permission handlers, it's recommended to register a single "broker" or "factory" implementation of `IWorkflowPermissionHandler`. This main handler would then use the `PermissionHandlerKey` (or other properties from the `WorkflowTrigger`) to delegate the `IsAuthorized` call to the appropriate internal logic or sub-handler.
    -   **Error Handling**:
        -   If `PermissionType` is `Handler` but no `IWorkflowPermissionHandler` is registered in the DI container, the `WorkflowEngine` will throw an error.
        -   If `PermissionType` is `Handler` but the `PermissionHandlerKey` on the trigger is null or empty, the `WorkflowEngine` will also throw an error.

-   **Database Storage Example for Trigger Permissions**:
    When using the `DatabaseWorkflowDefinitionProvider`, these permission settings are typically stored in the `WorkflowTriggers` table.
    -   The `PermissionType` column would store an integer representing the enum value (e.g., `0` for `Explicit`, `1` for `Hierarchy`, `2` for `Handler`).
    -   The `Permissions` column (primarily for `Explicit` type) would store the list of identifiers as a single string, often comma-separated (e.g., `"user1,role:Approvers,editorUser"`).
    -   The `PermissionHandlerKey` column (for `Handler` type) would store the key for your custom handler logic.

### Transitions

A workflow transition (`WorkflowTransition`) defines the allowed movement from one state to another via a specific trigger.
- `From` (string, required): The `StateKey` of the source state.
- `Trigger` (string, required): The `TriggerKey` that initiates this transition.
- `To` (string, required): The `StateKey` of the target state.
- `GuardKey` (string, optional): Key to resolve an `IWorkflowGuard` that can conditionally prevent the transition.

## History Store

The workflow engine uses an `IWorkflowHistoryStore` to log actions performed on workflow entities.
- By default, `AddWorkflowDbProvider()` includes a database-backed history store.
- For testing or simpler scenarios, an in-memory history store can be enabled: `AddSerenityWorkflow(o => o.UseInMemoryHistoryStore = true)`.
- History store implementations can record entries asynchronously. `RecordEntryAsync` and `RecordEntriesAsync` methods may optionally batch writes.

## Database Migrations

Database tables for workflow definitions, states, triggers, transitions, and history are created automatically during application startup if you use the `DatabaseWorkflowDefinitionProvider`. The provider's assembly includes FluentMigrator migrations for this purpose.

using Moq;
using Serenity.Workflow;
using System.Threading;
using System.Threading.Tasks;

namespace Serenity.Net.Tests.Workflow
{
    public class WorkflowEnginePermissionTests
    {
        private readonly Mock<IWorkflowDefinitionProvider> _mockDefinitionProvider;
        private readonly Mock<IUserAccessor> _mockUserAccessor;
        private readonly Mock<IWorkflowHistoryStore> _mockHistoryStore;
        private readonly Mock<IServiceProvider> _mockServiceProvider;
        private readonly Mock<IWorkflowPermissionHandler> _mockWorkflowPermissionHandler;
        private readonly WorkflowEngineOptions _workflowOptions;
        private readonly WorkflowEngine _engine;

        private const string TestWorkflowKey = "PermissionTestWorkflow";
        private const string TestTriggerKey = "DoPermittedAction";
        private const string TestInitialState = "Active";
        private const string TestTargetState = "Completed";
        private const string TestUser = "testuser";
        private const string AnotherUser = "anotheruser";

        public WorkflowEnginePermissionTests()
        {
            _mockDefinitionProvider = new Mock<IWorkflowDefinitionProvider>();
            _mockUserAccessor = new Mock<IUserAccessor>();
            _mockHistoryStore = new Mock<IWorkflowHistoryStore>();
            _mockServiceProvider = new Mock<IServiceProvider>();
            _mockWorkflowPermissionHandler = new Mock<IWorkflowPermissionHandler>();
            _workflowOptions = new WorkflowEngineOptions();

            // Default setup for IServiceProvider
            _mockServiceProvider.Setup(s => s.GetService(typeof(IWorkflowPermissionHandler)))
                .Returns(_mockWorkflowPermissionHandler.Object);
            _mockServiceProvider.Setup(s => s.GetService(typeof(IUserAccessor)))
                .Returns(_mockUserAccessor.Object);
            // For other types, return null by default, can be overridden in specific tests if needed.
            _mockServiceProvider.Setup(s => s.GetService(It.IsNotIn(typeof(IWorkflowPermissionHandler)))).Returns<Type>(null);



            _engine = new WorkflowEngine(
                _mockDefinitionProvider.Object,
                _mockServiceProvider.Object,
                _mockHistoryStore.Object,
                _workflowOptions,
                _mockUserAccessor.Object);

            _mockHistoryStore.Setup(hs => hs.RecordEntryAsync(It.IsAny<WorkflowHistoryEntry>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);
        }

        private WorkflowDefinition CreateWorkflowDefinition(WorkflowTrigger trigger, WorkflowState state, WorkflowTransition transition)
        {
            return new WorkflowDefinition
            {
                WorkflowKey = TestWorkflowKey,
                InitialState = state.StateKey,
                States = new Dictionary<string, WorkflowState> { { state.StateKey, state } },
                Triggers = new Dictionary<string, WorkflowTrigger> { { trigger.TriggerKey, trigger } },
                Transitions = new List<WorkflowTransition> { transition }
            };
        }

        private void SetupUser(string userId)
        {
            if (!string.IsNullOrEmpty(userId))
            {
                var claims = new[] { new Claim(ClaimTypes.NameIdentifier, userId) };
                var identity = new ClaimsIdentity(claims, "TestAuth");
                _mockUserAccessor.Setup(x => x.User).Returns(new ClaimsPrincipal(identity));
            }
            else
            {
                // Simulate unauthenticated user by returning a ClaimsPrincipal with no identity or an unauthenticated identity
                 var identity = new ClaimsIdentity(); // Unauthenticated identity
                _mockUserAccessor.Setup(x => x.User).Returns(new ClaimsPrincipal(identity));
                // Or return null if GetIdentifier() handles _userAccessor.User being null:
                // _mockUserAccessor.Setup(x => x.User).Returns((ClaimsPrincipal)null);
            }
        }

        private void SetupUserNotAuthenticatedAtAll()
        {
            _mockUserAccessor.Setup(x => x.User).Returns((ClaimsPrincipal)null);
        }


        private void SetupDefinitionForTrigger(WorkflowTrigger trigger)
        {
            var state = new WorkflowState { StateKey = TestInitialState, DisplayName = "Initial State" };
            var transition = new WorkflowTransition { From = TestInitialState, To = TestTargetState, Trigger = trigger.TriggerKey };
            var definition = CreateWorkflowDefinition(trigger, state, transition);
            _mockDefinitionProvider.Setup(x => x.GetDefinition(TestWorkflowKey)).Returns(definition);
        }

        [Fact]
        public async Task ExecuteAsync_ExplicitPermission_UserAllowed_ShouldSucceed()
        {
            var trigger = new WorkflowTrigger
            {
                TriggerKey = TestTriggerKey,
                PermissionType = PermissionGrantType.Explicit,
                Permissions = new List<string> { TestUser },
                HandlerKey = null // Ensure handler resolution is skipped
            };
            SetupDefinitionForTrigger(trigger);
            SetupUser(TestUser);

            await _engine.ExecuteAsync(TestWorkflowKey, TestInitialState, TestTriggerKey, null);
            // No exception indicates success
        }

        [Fact]
        public async Task ExecuteAsync_ExplicitPermission_UserDenied_NotInList_ShouldThrowValidationError()
        {
            var trigger = new WorkflowTrigger
            {
                TriggerKey = TestTriggerKey,
                PermissionType = PermissionGrantType.Explicit,
                Permissions = new List<string> { AnotherUser },
                HandlerKey = null
            };
            SetupDefinitionForTrigger(trigger);
            SetupUser(TestUser);

            var ex = await Assert.ThrowsAsync<ValidationError>(async () =>
                await _engine.ExecuteAsync(TestWorkflowKey, TestInitialState, TestTriggerKey, null));
            Assert.Contains("Not authorized to execute this action.", ex.Message);
        }

        [Fact]
        public async Task ExecuteAsync_ExplicitPermission_UserDenied_PermissionsListEmpty_ShouldThrowValidationError()
        {
            var trigger = new WorkflowTrigger
            {
                TriggerKey = TestTriggerKey,
                PermissionType = PermissionGrantType.Explicit,
                Permissions = new List<string>(), // Empty list
                HandlerKey = null
            };
            SetupDefinitionForTrigger(trigger);
            SetupUser(TestUser);

            var ex = await Assert.ThrowsAsync<ValidationError>(async () =>
                await _engine.ExecuteAsync(TestWorkflowKey, TestInitialState, TestTriggerKey, null));
            Assert.Contains("Not authorized to execute this action.", ex.Message);
        }

        [Fact]
        public async Task ExecuteAsync_ExplicitPermission_UserDenied_PermissionsListNull_ShouldThrowValidationError()
        {
            var trigger = new WorkflowTrigger
            {
                TriggerKey = TestTriggerKey,
                PermissionType = PermissionGrantType.Explicit,
                HandlerKey = null
                // Permissions property is initialized by default, so we manually set it to null
            };
            trigger.Permissions = null!; // Force null for test case

            SetupDefinitionForTrigger(trigger);
            SetupUser(TestUser);

            var ex = await Assert.ThrowsAsync<ValidationError>(async () =>
                await _engine.ExecuteAsync(TestWorkflowKey, TestInitialState, TestTriggerKey, null));
            Assert.Contains("Not authorized to execute this action.", ex.Message);
        }

        [Fact]
        public async Task ExecuteAsync_ExplicitPermission_UserDenied_UserNotAuthenticated_ShouldThrowValidationError()
        {
            var trigger = new WorkflowTrigger
            {
                TriggerKey = TestTriggerKey,
                PermissionType = PermissionGrantType.Explicit,
                Permissions = new List<string> { TestUser },
                HandlerKey = null
            };
            SetupDefinitionForTrigger(trigger);
            // SetupUser(null); // Simulate user not having an identifier / unauthenticated
            SetupUserNotAuthenticatedAtAll(); // Simulate _userAccessor.User being null

            var ex = await Assert.ThrowsAsync<ValidationError>(async () =>
                await _engine.ExecuteAsync(TestWorkflowKey, TestInitialState, TestTriggerKey, null));
            Assert.Contains("User not authenticated or identifier not available.", ex.Message);
        }

        [Fact]
        public async Task ExecuteAsync_ExplicitPermission_UserDenied_UserIdentifierNullOrEmpty_ShouldThrowValidationError()
        {
            var trigger = new WorkflowTrigger
            {
                TriggerKey = TestTriggerKey,
                PermissionType = PermissionGrantType.Explicit,
                Permissions = new List<string> { TestUser },
                HandlerKey = null
            };
            SetupDefinitionForTrigger(trigger);
            SetupUser(""); // Simulate user with empty identifier

            var ex = await Assert.ThrowsAsync<ValidationError>(async () =>
                await _engine.ExecuteAsync(TestWorkflowKey, TestInitialState, TestTriggerKey, null));
            Assert.Contains("User not authenticated or identifier not available.", ex.Message);
        }


        // --- Tests for PermissionGrantType.Handler ---

        [Fact]
        public async Task ExecuteAsync_HandlerPermission_HandlerAllows_ShouldSucceed()
        {
            var testEntity = new object();
            var trigger = new WorkflowTrigger
            {
                TriggerKey = TestTriggerKey,
                PermissionType = PermissionGrantType.Handler,
                PermissionHandlerKey = "TestSpecificHandlerRule", // Key provided
                HandlerKey = null
            };
            SetupDefinitionForTrigger(trigger);
            SetupUser(TestUser);

            _mockWorkflowPermissionHandler.Setup(h => h.IsAuthorized(
                _mockUserAccessor.Object.User, // Pass the actual ClaimsPrincipal from the mock
                trigger,
                testEntity,
                _mockServiceProvider.Object))
                .Returns(true);

            await _engine.ExecuteAsync(TestWorkflowKey, TestInitialState, TestTriggerKey,
                new Dictionary<string, object?> { { "Entity", testEntity } });

            _mockWorkflowPermissionHandler.Verify(h => h.IsAuthorized(
                _mockUserAccessor.Object.User, trigger, testEntity, _mockServiceProvider.Object), Times.Once);
        }

        [Fact]
        public async Task ExecuteAsync_HandlerPermission_HandlerDenies_ShouldThrowValidationError()
        {
            var testEntity = new object();
            var trigger = new WorkflowTrigger
            {
                TriggerKey = TestTriggerKey,
                PermissionType = PermissionGrantType.Handler,
                PermissionHandlerKey = "TestSpecificHandlerRule",
                HandlerKey = null
            };
            SetupDefinitionForTrigger(trigger);
            SetupUser(TestUser);

            _mockWorkflowPermissionHandler.Setup(h => h.IsAuthorized(
                _mockUserAccessor.Object.User, trigger, testEntity, _mockServiceProvider.Object))
                .Returns(false);

            var ex = await Assert.ThrowsAsync<ValidationError>(async () =>
                await _engine.ExecuteAsync(TestWorkflowKey, TestInitialState, TestTriggerKey,
                    new Dictionary<string, object?> { { "Entity", testEntity } }));

            Assert.Contains($"Not authorized by permission handler for trigger '{TestTriggerKey}'.", ex.Message);
            _mockWorkflowPermissionHandler.Verify(h => h.IsAuthorized(
                _mockUserAccessor.Object.User, trigger, testEntity, _mockServiceProvider.Object), Times.Once);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public async Task ExecuteAsync_HandlerPermission_HandlerKeyMissing_ShouldThrowValidationError(string? handlerKey)
        {
            var trigger = new WorkflowTrigger
            {
                TriggerKey = TestTriggerKey,
                PermissionType = PermissionGrantType.Handler,
                PermissionHandlerKey = handlerKey, // Key is missing
                HandlerKey = null
            };
            SetupDefinitionForTrigger(trigger);
            SetupUser(TestUser);

            var ex = await Assert.ThrowsAsync<ValidationError>(async () =>
                await _engine.ExecuteAsync(TestWorkflowKey, TestInitialState, TestTriggerKey, null));

            Assert.Contains("Permission handler key is missing for handler-based permission.", ex.Message);
        }

        [Fact]
        public async Task ExecuteAsync_HandlerPermission_NoHandlerRegistered_ShouldThrowValidationError()
        {
            var trigger = new WorkflowTrigger
            {
                TriggerKey = TestTriggerKey,
                PermissionType = PermissionGrantType.Handler,
                PermissionHandlerKey = "SomeKey", // Key is present
                HandlerKey = null
            };
            SetupDefinitionForTrigger(trigger);
            SetupUser(TestUser);

            // Override service provider setup for this test to simulate no handler registered
            _mockServiceProvider.Setup(s => s.GetService(typeof(IWorkflowPermissionHandler))).Returns(null);

            // Re-create engine with this specific provider setup if necessary, or ensure current engine uses this setup.
            // Current setup modifies the shared _mockServiceProvider, so it should apply.

            var ex = await Assert.ThrowsAsync<ValidationError>(async () =>
                await _engine.ExecuteAsync(TestWorkflowKey, TestInitialState, TestTriggerKey, null));

            Assert.Contains($"No IWorkflowPermissionHandler registered in DI. Cannot process handler-based permission for trigger '{TestTriggerKey}'.", ex.Message);

            // Reset for other tests if _mockServiceProvider is shared and modified
             _mockServiceProvider.Setup(s => s.GetService(typeof(IWorkflowPermissionHandler)))
                .Returns(_mockWorkflowPermissionHandler.Object);
        }

        [Fact]
        public async Task ExecuteAsync_HandlerPermission_HandlerThrowsException_ShouldPropagateException()
        {
            var testEntity = new object();
            var customException = new InvalidOperationException("Handler specific error");
            var trigger = new WorkflowTrigger
            {
                TriggerKey = TestTriggerKey,
                PermissionType = PermissionGrantType.Handler,
                PermissionHandlerKey = "TestSpecificHandlerRule",
                HandlerKey = null
            };
            SetupDefinitionForTrigger(trigger);
            SetupUser(TestUser);

            _mockWorkflowPermissionHandler.Setup(h => h.IsAuthorized(
                _mockUserAccessor.Object.User, trigger, testEntity, _mockServiceProvider.Object))
                .Throws(customException);

            var ex = await Assert.ThrowsAsync<InvalidOperationException>(async () =>
                await _engine.ExecuteAsync(TestWorkflowKey, TestInitialState, TestTriggerKey,
                    new Dictionary<string, object?> { { "Entity", testEntity } }));

            Assert.Same(customException, ex);
            _mockWorkflowPermissionHandler.Verify(h => h.IsAuthorized(
                _mockUserAccessor.Object.User, trigger, testEntity, _mockServiceProvider.Object), Times.Once);
        }
    }
}

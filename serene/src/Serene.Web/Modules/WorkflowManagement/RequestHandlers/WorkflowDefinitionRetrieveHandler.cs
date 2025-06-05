using Serenity.Services;
using MyRow = Serenity.Workflow.Entities.WorkflowDefinitionRow;
using MyRequest = RetrieveRequest;
using MyResponse = RetrieveResponse<Serenity.Workflow.Entities.WorkflowDefinitionRow>;

namespace Serene.WorkflowManagement;

public interface IWorkflowDefinitionRetrieveHandler : IRetrieveHandler<MyRow, MyRequest, MyResponse> { }

public class WorkflowDefinitionRetrieveHandler(IRequestContext context)
    : RetrieveRequestHandler<MyRow, MyRequest, MyResponse>(context), IWorkflowDefinitionRetrieveHandler
{
}

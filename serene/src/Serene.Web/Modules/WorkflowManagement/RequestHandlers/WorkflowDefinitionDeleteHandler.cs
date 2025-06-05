using Serenity.Services;
using MyRow = Serenity.Workflow.Entities.WorkflowDefinitionRow;
using MyRequest = DeleteRequest;
using MyResponse = DeleteResponse;

namespace Serene.WorkflowManagement;

public interface IWorkflowDefinitionDeleteHandler : IDeleteHandler<MyRow, MyRequest, MyResponse> { }

public class WorkflowDefinitionDeleteHandler(IRequestContext context)
    : DeleteRequestHandler<MyRow, MyRequest, MyResponse>(context), IWorkflowDefinitionDeleteHandler
{
}

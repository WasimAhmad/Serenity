using Serenity.Services;
using MyRow = Serenity.Workflow.Entities.WorkflowDefinitionRow;
using MyRequest = ListRequest;
using MyResponse = ListResponse<Serenity.Workflow.Entities.WorkflowDefinitionRow>;

namespace Serene.WorkflowManagement;

public interface IWorkflowDefinitionListHandler : IListHandler<MyRow, MyRequest, MyResponse> { }

public class WorkflowDefinitionListHandler(IRequestContext context)
    : ListRequestHandler<MyRow, MyRequest, MyResponse>(context), IWorkflowDefinitionListHandler
{
}

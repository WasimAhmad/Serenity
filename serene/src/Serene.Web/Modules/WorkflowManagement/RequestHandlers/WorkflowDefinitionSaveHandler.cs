using Serenity.Services;
using MyRow = Serenity.Workflow.Entities.WorkflowDefinitionRow;
using MyRequest = SaveRequest<Serenity.Workflow.Entities.WorkflowDefinitionRow>;
using MyResponse = SaveResponse;

namespace Serene.WorkflowManagement;

public interface IWorkflowDefinitionSaveHandler : ISaveHandler<MyRow, MyRequest, MyResponse> { }

public class WorkflowDefinitionSaveHandler(IRequestContext context)
    : SaveRequestHandler<MyRow, MyRequest, MyResponse>(context), IWorkflowDefinitionSaveHandler
{
}

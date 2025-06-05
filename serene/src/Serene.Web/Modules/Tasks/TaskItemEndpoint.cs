using MyRow = Serene.Tasks.TaskItemRow;

namespace Serene.Tasks.Endpoints;

[Route("Services/Tasks/TaskItem/[action]")]
[ConnectionKey(typeof(MyRow)), ServiceAuthorize(typeof(MyRow))]
public class TaskItemEndpoint : ServiceEndpoint
{
    [HttpPost, AuthorizeCreate(typeof(MyRow))]
    public SaveResponse Create(IUnitOfWork uow, SaveRequest<MyRow> request, [FromServices] ITaskItemSaveHandler handler)
    {
        return handler.Create(uow, request);
    }

    [HttpPost, AuthorizeUpdate(typeof(MyRow))]
    public SaveResponse Update(IUnitOfWork uow, SaveRequest<MyRow> request, [FromServices] ITaskItemSaveHandler handler)
    {
        return handler.Update(uow, request);
    }

    [HttpPost, AuthorizeDelete(typeof(MyRow))]
    public DeleteResponse Delete(IUnitOfWork uow, DeleteRequest request, [FromServices] ITaskItemDeleteHandler handler)
    {
        return handler.Delete(uow, request);
    }

    public RetrieveResponse<MyRow> Retrieve(IDbConnection connection, RetrieveRequest request, [FromServices] ITaskItemRetrieveHandler handler)
    {
        return handler.Retrieve(connection, request);
    }

    public ListResponse<MyRow> List(IDbConnection connection, ListRequest request, [FromServices] ITaskItemListHandler handler)
    {
        return handler.List(connection, request);
    }
}

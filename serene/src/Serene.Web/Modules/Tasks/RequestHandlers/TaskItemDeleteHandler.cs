using MyRow = Serene.Tasks.TaskItemRow;
using MyRequest = Serenity.Services.DeleteRequest;
using MyResponse = Serenity.Services.DeleteResponse;

namespace Serene.Tasks;

public interface ITaskItemDeleteHandler : IDeleteHandler<MyRow, MyRequest, MyResponse> { }

public class TaskItemDeleteHandler(IRequestContext context)
    : DeleteRequestHandler<MyRow, MyRequest, MyResponse>(context), ITaskItemDeleteHandler
{
}

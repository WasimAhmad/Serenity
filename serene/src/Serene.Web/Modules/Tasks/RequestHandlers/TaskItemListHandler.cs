using MyRow = Serene.Tasks.TaskItemRow;
using MyRequest = Serenity.Services.ListRequest;
using MyResponse = Serenity.Services.ListResponse<Serene.Tasks.TaskItemRow>;

namespace Serene.Tasks;

public interface ITaskItemListHandler : IListHandler<MyRow, MyRequest, MyResponse> { }

public class TaskItemListHandler(IRequestContext context)
    : ListRequestHandler<MyRow, MyRequest, MyResponse>(context), ITaskItemListHandler
{
}

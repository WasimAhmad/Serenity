using MyRow = Serene.Tasks.TaskItemRow;
using MyRequest = Serenity.Services.RetrieveRequest;
using MyResponse = Serenity.Services.RetrieveResponse<Serene.Tasks.TaskItemRow>;

namespace Serene.Tasks;

public interface ITaskItemRetrieveHandler : IRetrieveHandler<MyRow, MyRequest, MyResponse> { }

public class TaskItemRetrieveHandler(IRequestContext context)
    : RetrieveRequestHandler<MyRow, MyRequest, MyResponse>(context), ITaskItemRetrieveHandler
{
}

using MyRow = Serene.Tasks.TaskItemRow;
using MyRequest = Serenity.Services.SaveRequest<Serene.Tasks.TaskItemRow>;
using MyResponse = Serenity.Services.SaveResponse;

namespace Serene.Tasks;

public interface ITaskItemSaveHandler : ISaveHandler<MyRow, MyRequest, MyResponse> { }

public class TaskItemSaveHandler(IRequestContext context)
    : SaveRequestHandler<MyRow, MyRequest, MyResponse>(context), ITaskItemSaveHandler
{
}

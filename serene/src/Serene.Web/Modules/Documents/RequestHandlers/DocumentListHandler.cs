using MyRow = Serene.Documents.DocumentRow;
using MyRequest = Serenity.Services.ListRequest;
using MyResponse = Serenity.Services.ListResponse<Serene.Documents.DocumentRow>;

namespace Serene.Documents;

public interface IDocumentListHandler : IListHandler<MyRow, MyRequest, MyResponse> { }

public class DocumentListHandler(IRequestContext context)
    : ListRequestHandler<MyRow, MyRequest, MyResponse>(context), IDocumentListHandler
{
}

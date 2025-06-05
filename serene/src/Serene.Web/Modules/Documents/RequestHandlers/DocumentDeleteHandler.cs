using MyRow = Serene.Documents.DocumentRow;
using MyRequest = Serenity.Services.DeleteRequest;
using MyResponse = Serenity.Services.DeleteResponse;

namespace Serene.Documents;

public interface IDocumentDeleteHandler : IDeleteHandler<MyRow, MyRequest, MyResponse> { }

public class DocumentDeleteHandler(IRequestContext context)
    : DeleteRequestHandler<MyRow, MyRequest, MyResponse>(context), IDocumentDeleteHandler
{
}

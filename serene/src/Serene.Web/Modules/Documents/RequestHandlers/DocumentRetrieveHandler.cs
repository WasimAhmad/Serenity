using MyRow = Serene.Documents.DocumentRow;
using MyRequest = Serenity.Services.RetrieveRequest;
using MyResponse = Serenity.Services.RetrieveResponse<Serene.Documents.DocumentRow>;

namespace Serene.Documents;

public interface IDocumentRetrieveHandler : IRetrieveHandler<MyRow, MyRequest, MyResponse> { }

public class DocumentRetrieveHandler(IRequestContext context)
    : RetrieveRequestHandler<MyRow, MyRequest, MyResponse>(context), IDocumentRetrieveHandler
{
}

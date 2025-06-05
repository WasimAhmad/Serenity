using MyRow = Serene.Documents.DocumentRow;
using MyRequest = Serenity.Services.SaveRequest<Serene.Documents.DocumentRow>;
using MyResponse = Serenity.Services.SaveResponse;

namespace Serene.Documents;

public interface IDocumentSaveHandler : ISaveHandler<MyRow, MyRequest, MyResponse> { }

public class DocumentSaveHandler(IRequestContext context)
    : SaveRequestHandler<MyRow, MyRequest, MyResponse>(context), IDocumentSaveHandler
{
}

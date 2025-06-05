namespace Serene.Documents.Pages;

[PageAuthorize(typeof(DocumentRow))]
public class DocumentPage : Controller
{
    [Route("Documents/Document")]
    public ActionResult Index()
    {
        return this.GridPage("@/Documents/DocumentPage", DocumentRow.Fields.PageTitle());
    }
}

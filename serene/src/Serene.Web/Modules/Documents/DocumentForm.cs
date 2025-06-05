namespace Serene.Documents.Forms;

[FormScript("Documents.Document")]
[BasedOnRow(typeof(DocumentRow), CheckNames = true)]
public class DocumentForm
{
    public string Title { get; set; }
    public string State { get; set; }
}

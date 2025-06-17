namespace Serene.Documents.Forms;

[FormScript("Documents.Document")]
[BasedOnRow(typeof(DocumentRow), CheckNames = true)]
public class DocumentForm
{
    public string Title { get; set; }

    [Updatable(false)]
    public DocumentType? DocumentType { get; set; }

    [DefaultValue("Draft"), ReadOnly(true)]
    public string State { get; set; }
}

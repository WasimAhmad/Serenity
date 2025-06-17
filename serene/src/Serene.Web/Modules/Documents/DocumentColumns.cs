namespace Serene.Documents.Forms;

[ColumnsScript("Documents.Document")]
[BasedOnRow(typeof(DocumentRow), CheckNames = true)]
public class DocumentColumns
{
    [EditLink]
    public string Title { get; set; }
    public DocumentType DocumentType { get; set; }
    public string State { get; set; }
}

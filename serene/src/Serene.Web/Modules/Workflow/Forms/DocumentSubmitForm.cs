using Serene.Administration;

namespace Serene.Workflow.Forms;

[FormScript("Workflow.DocumentSubmit")]
public class DocumentSubmitForm
{
    [LookupEditor(typeof(LanguageRow))]
    public int Language { get; set; }
    [TextAreaEditor(Rows = 3)]
    public string Comment { get; set; }
}

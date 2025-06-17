using Serenity.Workflow;

namespace Serene.Documents;

[ConnectionKey("Default"), Module("Documents"), TableName("Documents")]
[DisplayName("Documents"), InstanceName("Document")]
[ReadPermission("Document:View")]
//[InsertPermission("Document:View")]
//[UpdatePermission("Document:View")]
//[DeletePermission("Document:View")]

[ModifyPermission("Document:Modify")]
//[WorkflowEnabled("DocumentWorkflow")]
public sealed class DocumentRow : Row<DocumentRow.RowFields>, IIdRow, INameRow
{
    [DisplayName("Document Id"), Identity, IdProperty]
    public int? DocumentId { get => fields.DocumentId[this]; set => fields.DocumentId[this] = value; }

    [DisplayName("Title"), Size(100), NotNull, QuickSearch, NameProperty]
    public string Title { get => fields.Title[this]; set => fields.Title[this] = value; }


    [DisplayName("Type"), NotNull]
    public DocumentType? DocumentType
    {
        get => fields.DocumentType[this];
        set => fields.DocumentType[this] = value;
    }

    //[DisplayName("Type"), NotNull]
    //public DocumentType? DocumentType { get => (DocumentType?)fields.DocumentType[this]; set => fields.DocumentType[this] = value; }

    [DisplayName("State"), Size(50), NotNull]
    //[WorkflowStateField]
    public string State { get => fields.State[this]; set => fields.State[this] = value; }

    public class RowFields : RowFieldsBase
    {
        public Int32Field DocumentId;
        public StringField Title;
        public StringField State;
        public EnumField<DocumentType> DocumentType;
    }
}

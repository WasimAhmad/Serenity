using Serenity.ComponentModel;
using Serenity.Data;
using Serenity.Data.Mapping;

namespace Serene.Web.Workflow.DbProvider.Entities
{
    [ConnectionKey("Default"), Module("Workflow"), TableName("WorkflowDefinitions")]
    [DisplayName("Workflow Definitions"), InstanceName("Workflow Definition")]
    [ReadPermission("Workflow:View"), ModifyPermission("Workflow:Modify")]
    public class WorkflowDefinitionRow : Row<WorkflowDefinitionRow.RowFields>
    {
        [Identity, IdProperty]
        public int? Id { get => fields.Id[this]; set => fields.Id[this] = value; }
        public string? WorkflowKey { get => fields.WorkflowKey[this]; set => fields.WorkflowKey[this] = value; }
        [NameProperty]
        public string? Name { get => fields.Name[this]; set => fields.Name[this] = value; }
        public string? InitialState { get => fields.InitialState[this]; set => fields.InitialState[this] = value; }


        public class RowFields : RowFieldsBase
        {
            public Int32Field Id;
            public StringField WorkflowKey;
            public StringField Name;
            public StringField InitialState;
        }
    }
}

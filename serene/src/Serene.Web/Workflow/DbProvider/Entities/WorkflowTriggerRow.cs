using Serenity.ComponentModel;
using Serenity.Data;
using Serenity.Data.Mapping;

namespace Serene.Web.Workflow.DbProvider.Entities
{
    [ConnectionKey("Default"), TableName("WorkflowTriggers")]
    public class WorkflowTriggerRow : Row<WorkflowTriggerRow.RowFields>
    {
        [Identity, IdProperty]
        public int? Id { get => fields.Id[this]; set => fields.Id[this] = value; }
        public int? DefinitionId { get => fields.DefinitionId[this]; set => fields.DefinitionId[this] = value; }
        public string? TriggerKey { get => fields.TriggerKey[this]; set => fields.TriggerKey[this] = value; }
        public string? Name { get => fields.Name[this]; set => fields.Name[this] = value; }
        public string? HandlerKey { get => fields.HandlerKey[this]; set => fields.HandlerKey[this] = value; }
        public bool? RequiresInput { get => fields.RequiresInput[this]; set => fields.RequiresInput[this] = value; }
        public string? FormKey { get => fields.FormKey[this]; set => fields.FormKey[this] = value; }

        public class RowFields : RowFieldsBase
        {
            public Int32Field Id;
            public Int32Field DefinitionId;
            public StringField TriggerKey;
            public StringField Name;
            public StringField HandlerKey;
            public BooleanField RequiresInput;
            public StringField FormKey;
        }
    }
}

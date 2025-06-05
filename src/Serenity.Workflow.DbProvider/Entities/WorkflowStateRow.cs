using Serenity.ComponentModel;
using Serenity.Data;
using Serenity.Data.Mapping;

namespace Serenity.Workflow.Entities
{
    [ConnectionKey("Default"), TableName("WorkflowStates")]
    public class WorkflowStateRow : Row<WorkflowStateRow.RowFields>
    {
        [Identity, IdProperty]
        public int? Id { get => fields.Id[this]; set => fields.Id[this] = value; }
        public int? DefinitionId { get => fields.DefinitionId[this]; set => fields.DefinitionId[this] = value; }
        public string? StateKey { get => fields.StateKey[this]; set => fields.StateKey[this] = value; }
        public string? Name { get => fields.Name[this]; set => fields.Name[this] = value; }

        public class RowFields : RowFieldsBase
        {
            public Int32Field Id;
            public Int32Field DefinitionId;
            public StringField StateKey;
            public StringField Name;
        }
    }
}

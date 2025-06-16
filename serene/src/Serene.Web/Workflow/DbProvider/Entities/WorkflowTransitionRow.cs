using Serenity.ComponentModel;
using Serenity.Data;
using Serenity.Data.Mapping;

namespace Serene.Web.Workflow.DbProvider.Entities
{
    [ConnectionKey("Default"), TableName("WorkflowTransitions")]
    public class WorkflowTransitionRow : Row<WorkflowTransitionRow.RowFields>
    {
        [Identity, IdProperty]
        public int? Id { get => fields.Id[this]; set => fields.Id[this] = value; }
        public int? DefinitionId { get => fields.DefinitionId[this]; set => fields.DefinitionId[this] = value; }
        public string? FromState { get => fields.FromState[this]; set => fields.FromState[this] = value; }
        public string? ToState { get => fields.ToState[this]; set => fields.ToState[this] = value; }
        public string? TriggerKey { get => fields.TriggerKey[this]; set => fields.TriggerKey[this] = value; }
        public string? GuardKey { get => fields.GuardKey[this]; set => fields.GuardKey[this] = value; }

        public class RowFields : RowFieldsBase
        {
            public Int32Field Id;
            public Int32Field DefinitionId;
            public StringField FromState;
            public StringField ToState;
            public StringField TriggerKey;
            public StringField GuardKey;
        }
    }
}

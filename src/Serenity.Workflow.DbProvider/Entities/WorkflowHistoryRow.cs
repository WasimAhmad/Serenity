using Serenity.ComponentModel;
using Serenity.Data;
using Serenity.Data.Mapping;
using System;

namespace Serenity.Workflow.Entities;

[ConnectionKey("Default"), TableName("WorkflowHistory")]
public class WorkflowHistoryRow : Row<WorkflowHistoryRow.RowFields>
{
    [Identity, IdProperty]
    public int? Id { get => fields.Id[this]; set => fields.Id[this] = value; }
    public string? WorkflowKey { get => fields.WorkflowKey[this]; set => fields.WorkflowKey[this] = value; }
    public string? EntityId { get => fields.EntityId[this]; set => fields.EntityId[this] = value; }
    public string? FromState { get => fields.FromState[this]; set => fields.FromState[this] = value; }
    public string? ToState { get => fields.ToState[this]; set => fields.ToState[this] = value; }
    public string? Trigger { get => fields.Trigger[this]; set => fields.Trigger[this] = value; }
    public string? Input { get => fields.Input[this]; set => fields.Input[this] = value; }
    public DateTime? EventDate { get => fields.EventDate[this]; set => fields.EventDate[this] = value; }
    public string? User { get => fields.User[this]; set => fields.User[this] = value; }

    public class RowFields : RowFieldsBase
    {
        public Int32Field Id;
        public StringField WorkflowKey;
        public StringField EntityId;
        public StringField FromState;
        public StringField ToState;
        public StringField Trigger;
        public StringField Input;
        public DateTimeField EventDate;
        public StringField User;
    }
}

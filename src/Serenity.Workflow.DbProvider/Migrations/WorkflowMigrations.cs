using FluentMigrator;
using Serenity.Extensions;

namespace Serenity.Workflow.Migrations
{
    [DefaultDB, MigrationKey(20250605_1405)]
    public class WorkflowMigrations : Migration
    {
        public override void Up()
        {
            Create.Table("WorkflowDefinitions")
                .WithColumn("Id").AsInt32().PrimaryKey().Identity()
                .WithColumn("WorkflowKey").AsString(100).NotNullable().Unique()
                .WithColumn("Name").AsString(100).NotNullable()
                .WithColumn("InitialState").AsString(100).NotNullable();

            Create.Table("WorkflowStates")
                .WithColumn("Id").AsInt32().PrimaryKey().Identity()
                .WithColumn("DefinitionId").AsInt32().NotNullable().Indexed()
                .WithColumn("StateKey").AsString(100).NotNullable()
                .WithColumn("Name").AsString(100).NotNullable();

            Create.Table("WorkflowTriggers")
                .WithColumn("Id").AsInt32().PrimaryKey().Identity()
                .WithColumn("DefinitionId").AsInt32().NotNullable().Indexed()
                .WithColumn("TriggerKey").AsString(100).NotNullable()
                .WithColumn("Name").AsString(100).NotNullable()
                .WithColumn("HandlerKey").AsString(200).Nullable()
                .WithColumn("RequiresInput").AsBoolean().WithDefaultValue(false)
                .WithColumn("FormKey").AsString(200).Nullable();

            Create.Table("WorkflowTransitions")
                .WithColumn("Id").AsInt32().PrimaryKey().Identity()
                .WithColumn("DefinitionId").AsInt32().NotNullable().Indexed()
                .WithColumn("FromState").AsString(100).NotNullable()
                .WithColumn("ToState").AsString(100).NotNullable()
                .WithColumn("TriggerKey").AsString(100).NotNullable()
                .WithColumn("GuardKey").AsString(200).Nullable();
        }

        public override void Down()
        {
            Delete.Table("WorkflowTransitions");
            Delete.Table("WorkflowTriggers");
            Delete.Table("WorkflowStates");
            Delete.Table("WorkflowDefinitions");
        }
    }
}

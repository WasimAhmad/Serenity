using FluentMigrator;
using Serenity.Extensions;

namespace Serene.Web.Workflow.DbProvider.Migrations;

[DefaultDB, MigrationKey(20250605_1406)]
public class WorkflowHistoryMigrations : Migration
{
    public override void Up()
    {
        Create.Table("WorkflowHistory")
            .WithColumn("Id").AsInt32().PrimaryKey().Identity()
            .WithColumn("WorkflowKey").AsString(100).NotNullable()
            .WithColumn("EntityId").AsString(100).NotNullable()
            .WithColumn("FromState").AsString(100).NotNullable()
            .WithColumn("ToState").AsString(100).NotNullable()
            .WithColumn("Trigger").AsString(100).NotNullable()
            .WithColumn("Input").AsString(int.MaxValue).Nullable()
            .WithColumn("EventDate").AsDateTime().NotNullable().WithDefault(SystemMethods.CurrentUTCDateTime)
            .WithColumn("User").AsString(100).Nullable();

        Create.Index("IX_WorkflowHistory_Workflow_Entity")
            .OnTable("WorkflowHistory")
            .OnColumn("WorkflowKey").Ascending()
            .OnColumn("EntityId").Ascending();
    }

    public override void Down()
    {
        Delete.Index("IX_WorkflowHistory_Workflow_Entity").OnTable("WorkflowHistory");
        Delete.Table("WorkflowHistory");
    }
}

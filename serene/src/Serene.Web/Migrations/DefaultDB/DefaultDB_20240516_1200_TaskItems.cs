using FluentMigrator;

namespace Serene.Migrations.DefaultDB;

[DefaultDB, MigrationKey(20240516_1200)]
public class DefaultDB_20240516_1200_TaskItems : AutoReversingMigration
{
    public override void Up()
    {
        Create.Table("TaskItems")
            .WithColumn("TaskId").AsInt32().Identity().PrimaryKey()
            .WithColumn("Title").AsString(100).NotNullable()
            .WithColumn("State").AsString(50).NotNullable();
    }
}

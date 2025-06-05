using FluentMigrator;

namespace Serene.Migrations.DefaultDB;

[DefaultDB, MigrationKey(20240516_1300)]
public class DefaultDB_20240516_1300_Documents : AutoReversingMigration
{
    public override void Up()
    {
        Create.Table("Documents")
            .WithColumn("DocumentId").AsInt32().Identity().PrimaryKey()
            .WithColumn("Title").AsString(100).NotNullable()
            .WithColumn("State").AsString(50).NotNullable();
    }
}

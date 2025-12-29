using FluentMigrator;

namespace Byndyusoft.Data.Relational.QueryBuilder.Sample.Migrations
{
    [Migration(20251228105600)]
    public class Migration20251228105600 : ForwardOnlyMigration
    {
        public override void Up()
        {
            Create.Table("money")
                .WithColumn("id").AsInt64().PrimaryKey().Identity()
                .WithColumn("currency").AsString(int.MaxValue).NotNullable()
                .WithColumn("value").AsDecimal().NotNullable();
        }
    }
}
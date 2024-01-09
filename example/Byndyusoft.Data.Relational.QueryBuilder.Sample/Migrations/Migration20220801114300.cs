using FluentMigrator;

namespace Byndyusoft.Data.Relational.QueryBuilder.Sample.Migrations
{
    [Migration(20220801114300)]
    public class Migration20220801114300 : ForwardOnlyMigration
    {
        public override void Up()
        {
            Create.Table("companies")
                .WithColumn("id").AsInt64().PrimaryKey().Identity()
                .WithColumn("name").AsString(int.MaxValue).NotNullable().Indexed()
                .WithColumn("inn").AsString(int.MaxValue).NotNullable();

            Create.Table("users")
                .WithColumn("id").AsInt64().PrimaryKey().Identity()
                .WithColumn("login").AsString(int.MaxValue).NotNullable()
                .WithColumn("password").AsString(int.MaxValue).NotNullable()
                .WithColumn("company_id").AsInt64().ForeignKey("companies", "id");

            Insert.IntoTable("companies")
                .Row(new { name = "Poor", inn = "777777777" })
                .Row(new { name = "Small", inn = "777777778" })
                .Row(new { name = "Big", inn = "777777779" });

            Execute.Sql(@"
INSERT INTO users(login, password, company_id)
SELECT 'small_employee', 'pwd1', c.id
FROM companies c
WHERE c.name = 'Small';

INSERT INTO users(login, password, company_id)
SELECT 'big_employee', 'pwd21', c.id
FROM companies c
WHERE c.name = 'Big';

INSERT INTO users(login, password, company_id)
SELECT 'big_manager', 'pwd22', c.id
FROM companies c
WHERE c.name = 'Big';");
        }
    }
}
using FluentMigrator;

namespace Route256.Week5.Workshop.PriceCalculator.Dal.Migrations;

[Migration(20240412153800, TransactionBehavior.None)]
public class AddColumnDeletedAdToTaskComments : Migration
{
    public override void Up()
    {
        const string sqlQuery = @"
alter table task_comments
add column if not exists deleted_at timestamp with time zone null;
";
        Execute.Sql(sqlQuery);
    }

    public override void Down()
    {
        const string sqlQuery = @"
alter table task_comments
drop column deleted_at;
";
        Execute.Sql(sqlQuery);
    }
}
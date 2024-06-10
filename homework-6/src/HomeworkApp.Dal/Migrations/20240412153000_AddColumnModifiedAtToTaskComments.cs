using FluentMigrator;

namespace Route256.Week5.Workshop.PriceCalculator.Dal.Migrations;

[Migration(20240412153000, TransactionBehavior.None)]
public class AddColumnModifiedAtToTaskComments : Migration
{
    public override void Up()
    {
        const string sqlQuery = @"
alter table task_comments
add column if not exists modified_at timestamp with time zone null;
";
        Execute.Sql(sqlQuery);
    }

    public override void Down()
    {
        const string sqlQuery = @"
alter table task_comments
drop column modified_at;
";
        Execute.Sql(sqlQuery);
    }
}
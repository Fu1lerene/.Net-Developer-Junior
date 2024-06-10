using System;
using FluentMigrator;
using Ozon.Route256.Kafka.OrderEventConsumer.Infrastructure.Common;

namespace Ozon.Route256.Postgres.Persistence.Migrations;

[Migration(3, "Add table orders")]
public class AddTableOrders : SqlMigration
{
    protected override string GetUpSql(IServiceProvider services) => @"
create table if not exists orders(
    order_id    bigint primary key  not null,
    status      integer             not null,   
    items_id    bigint[]            not null
);
";
}
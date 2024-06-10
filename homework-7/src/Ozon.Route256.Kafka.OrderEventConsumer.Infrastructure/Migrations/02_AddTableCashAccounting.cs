using System;
using FluentMigrator;
using Ozon.Route256.Kafka.OrderEventConsumer.Infrastructure.Common;

namespace Ozon.Route256.Postgres.Persistence.Migrations;

[Migration(2, "Add table cash accounting")]
public class AddTableCashAccounting : SqlMigration
{
    protected override string GetUpSql(IServiceProvider services) => @"
create table if not exists cash_accounting(
    seller_id   bigint                 not null,
    item_id     bigint                 not null,
    currency    character varying(3)   not null,
    quantity    bigint                 not null,
    price       decimal                not null,
    primary key (seller_id, item_id, currency)
);
";
}
using System;
using FluentMigrator;

using Ozon.Route256.Kafka.OrderEventConsumer.Infrastructure.Common;

namespace Ozon.Route256.Postgres.Persistence.Migrations;

[Migration(1, "Initial migration")]
public sealed class Initial : SqlMigration
{
    protected override string GetUpSql(IServiceProvider services) => @"
create table if not exists products(
    item_id     bigint primary key,
    created     bigint not null,
    delivered   bigint not null,
    cancelled   bigint not null,
    updated_at timestamp with time zone
);
";
}
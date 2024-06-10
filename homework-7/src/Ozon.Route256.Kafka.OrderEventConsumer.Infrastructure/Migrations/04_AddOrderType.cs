using System;
using FluentMigrator;
using Ozon.Route256.Kafka.OrderEventConsumer.Infrastructure.Common;

namespace Ozon.Route256.Postgres.Persistence.Migrations;

[Migration(4, "Add order type")]
public class AddOrderType : SqlMigration
{
    protected override string GetUpSql(IServiceProvider services) => @"
DO $$
    BEGIN
        IF NOT EXISTS (SELECT 1 FROM pg_type WHERE typname = 'orders_type') THEN
            CREATE TYPE orders_type as
            (
                   order_id    bigint        
                ,  status      integer          
                ,  items_id    bigint[]      
            );
        END IF;
    END
$$;
";
}
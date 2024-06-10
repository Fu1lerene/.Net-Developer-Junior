using System.Threading;
using System.Threading.Tasks;
using Dapper;
using Ozon.Route256.Kafka.OrderEventConsumer.Domain.Entities;
using Ozon.Route256.Kafka.OrderEventConsumer.Domain.Repositories;

namespace Ozon.Route256.Kafka.OrderEventConsumer.Infrastructure.Repositories;

public sealed class ItemRepository : PgRepository, IItemRepository
{
    private readonly string _connectionString;

    public ItemRepository(string connectionString) => _connectionString = connectionString;

    public async Task Add(ItemEntity[] items, CancellationToken token)
    {
        const string sqlQuery = @"
insert into products (item_id, created, delivered, cancelled, updated_at) 
values (@ItemId, @Created, @Delivered, @Cancelled, @UpdatedAt)
on conflict (item_id)
do update
   set created = products.created + @Created
     , delivered = products.delivered + @Delivered
     , cancelled = products.cancelled + @Cancelled
     , updated_at = @UpdatedAt
;";

        await using var connection = await GetConnection(_connectionString);
        
        await connection.ExecuteAsync(
            new CommandDefinition(
                sqlQuery,
                items,
                cancellationToken: token
                ));
    }
}

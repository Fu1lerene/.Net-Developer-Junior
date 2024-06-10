using System.Threading;
using System.Threading.Tasks;
using Dapper;
using Ozon.Route256.Kafka.OrderEventConsumer.Domain.Entities;
using Ozon.Route256.Kafka.OrderEventConsumer.Domain.Repositories;

namespace Ozon.Route256.Kafka.OrderEventConsumer.Infrastructure.Repositories;

public class ItemCashAccountingRepository : PgRepository, IItemCashAccountingRepository
{
    private readonly string _connectionString;

    public ItemCashAccountingRepository(string connectionString) => _connectionString = connectionString;

    public async Task Add(CashAccountEntity[] items, CancellationToken token)
    {
        const string sqlQuery = @"
insert into cash_accounting (seller_id, item_id, currency, quantity, price) 
values (@SellerId, @ItemId, @Currency, @Quantity, @Price)
on conflict (seller_id, item_id, currency)
do update
   set quantity = cash_accounting.quantity + @Quantity;
";

        await using var connection = await GetConnection(_connectionString);
        
        await connection.ExecuteAsync(
            new CommandDefinition(
                sqlQuery,
                items,
                cancellationToken: token
                ));
    }
}
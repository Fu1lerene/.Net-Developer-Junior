using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dapper;
using Ozon.Route256.Kafka.OrderEventConsumer.Domain.Entities;
using Ozon.Route256.Kafka.OrderEventConsumer.Domain.Repositories;

namespace Ozon.Route256.Kafka.OrderEventConsumer.Infrastructure.Repositories;

public class OrderRepository : PgRepository, IOrderRepository
{
    private readonly string _connectionString;

    public OrderRepository(string connectionString) => _connectionString = connectionString;

    public async Task Add(OrderEntity[] orders, CancellationToken token)
    {
        const string sqlQuery = @"
insert into orders (order_id, status, items_id) 
values (@OrderId, @Status, @ItemsId)
on conflict (order_id)
do nothing
;";

        await using var connection = await GetConnection(_connectionString);
        
        await connection.ExecuteAsync(
            new CommandDefinition(
                sqlQuery,
                orders,
                cancellationToken: token
                ));
    }
    
    public async Task<OrderEntity[]> GetByIds(long[] orderIds, CancellationToken token)
    {
        const string sqlQuery = @"
select order_id 
     , status   
     , items_id 
  from orders
";
        
        var conditions = new List<string>();
        var @params = new DynamicParameters();
        
        conditions.Add($"order_id = ANY(@OrderIds)");
        @params.Add($"OrderIds", orderIds);
        
        var cmd = new CommandDefinition(
            sqlQuery + $" WHERE {string.Join(" AND ", conditions)} ",
            @params,
            commandTimeout: DefaultTimeoutInSeconds,
            cancellationToken: token);
        
        await using var connection = await GetConnection(_connectionString);
        var orderEntities = (await connection.QueryAsync<OrderEntity>(cmd)).ToArray();
        
        return orderEntities;
    }
    
    public async Task<long[]> GetItemIdsByOrderId(long orderId, CancellationToken token)
    {
        const string sqlQuery = @"
select items_id 
  from orders
";
        
        var conditions = new List<string>();
        var @params = new DynamicParameters();
        
        conditions.Add($"order_id = ANY(@OrderIds)");
        @params.Add($"OrderIds", orderId);
        
        var cmd = new CommandDefinition(
            sqlQuery + $" WHERE {string.Join(" AND ", conditions)} ",
            @params,
            commandTimeout: DefaultTimeoutInSeconds,
            cancellationToken: token);
        
        await using var connection = await GetConnection(_connectionString);
        var orderEntities = (await connection.QueryAsync<long>(cmd)).ToArray();
        
        return orderEntities;
    }
}
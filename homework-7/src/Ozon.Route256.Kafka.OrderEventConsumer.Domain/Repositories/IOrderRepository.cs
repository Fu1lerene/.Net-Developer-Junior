using System.Threading;
using System.Threading.Tasks;
using Ozon.Route256.Kafka.OrderEventConsumer.Domain.Entities;

namespace Ozon.Route256.Kafka.OrderEventConsumer.Domain.Repositories;

public interface IOrderRepository
{
    Task Add(OrderEntity[] orders, CancellationToken token);

    Task<OrderEntity[]> GetByIds(long[] orderIds, CancellationToken token);

    Task<long[]> GetItemIdsByOrderId(long orderId, CancellationToken token);
}
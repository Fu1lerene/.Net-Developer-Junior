using System.Threading;
using System.Threading.Tasks;
using Ozon.Route256.Kafka.OrderEventConsumer.Domain.Entities;

namespace Ozon.Route256.Kafka.OrderEventConsumer.Domain.Repositories;

public interface IItemRepository
{
    Task Add(ItemEntity[] items, CancellationToken token);
}

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Ozon.Route256.Kafka.OrderEventConsumer.Domain.Order;

namespace Ozon.Route256.Kafka.OrderEventConsumer.Domain.Services;

public interface IOrderEventService
{
    Task AddItems(IEnumerable<OrderEvent> orderEvents, CancellationToken token);

    Task AddCashAccount(IEnumerable<OrderEvent> orderEvents, CancellationToken token);

    Task AddOrders(IEnumerable<OrderEvent> orderEvents, CancellationToken token);
}
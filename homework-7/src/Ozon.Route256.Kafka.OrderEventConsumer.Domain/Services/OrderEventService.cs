using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Ozon.Route256.Kafka.OrderEventConsumer.Domain.Common;
using Ozon.Route256.Kafka.OrderEventConsumer.Domain.Entities;
using Ozon.Route256.Kafka.OrderEventConsumer.Domain.Order;
using Ozon.Route256.Kafka.OrderEventConsumer.Domain.Repositories;

namespace Ozon.Route256.Kafka.OrderEventConsumer.Domain.Services;

public class OrderEventService : IOrderEventService
{
    private readonly IItemRepository _itemRepository;
    private readonly IItemCashAccountingRepository _accountingRepository;
    private readonly IOrderRepository _orderRepository;

    public OrderEventService(
        IItemRepository itemRepository,
        IItemCashAccountingRepository accountingRepository,
        IOrderRepository orderRepository)
    {
        _itemRepository = itemRepository;
        _accountingRepository = accountingRepository;
        _orderRepository = orderRepository;
    }

    public async Task AddItems(IEnumerable<OrderEvent> orderEvents, CancellationToken token)
    {
        var orders = await GetUnicOrders(orderEvents, token);
        var unicOrderEvents = OrderEventConverter.ToUnicOrderEvents(orderEvents, orders);
        
        var statuses = unicOrderEvents.GroupBy(x => x.Status);
        
        var itemEntities = new List<ItemEntity>();
        foreach (var status in statuses)
        {
            itemEntities
                .AddRange(unicOrderEvents
                .SelectMany(oe => oe.Positions)
                .Select(oep => OrderEventConverter.ToItemEntity(oep, status.Key)));
        }
        
        await _itemRepository.Add(itemEntities.ToArray(), token);
    }
    
    public async Task AddCashAccount(IEnumerable<OrderEvent> orderEvents, CancellationToken token)
    {
        var orders = await GetUnicOrders(orderEvents, token);
        var unicOrderEvents = OrderEventConverter.ToUnicOrderEvents(orderEvents, orders);
        
        var deliveredOrderEvents = unicOrderEvents
            .SelectMany(oe => oe.Positions)
            .Select(OrderEventConverter.ToCashAccountEntity);
        
        await _accountingRepository.Add(deliveredOrderEvents.ToArray(), token);
    }
    
    public async Task AddOrders(IEnumerable<OrderEvent> orderEvents, CancellationToken token)
    {
        var orders = (await GetUnicOrders(orderEvents, token)).ToArray();

        if (orders.Length == 0)
        {
            return;
        }

        await _orderRepository.Add(orders, token);
    }

    private async Task<IEnumerable<OrderEntity>> GetUnicOrders(IEnumerable<OrderEvent> orderEvents, CancellationToken token)
    {
        var orderIds = orderEvents.Select(oe => oe.OrderId.Value).ToArray();
        var orders = orderEvents
            .Select(OrderEventConverter.ToOrderEntity)
            .ToArray();
    
        var existingOrders =  await _orderRepository.GetByIds(orderIds, token);
    
        var intersect = orders
            .IntersectBy(existingOrders.Select(x => x.OrderId), x => x.OrderId)
            .IntersectBy(existingOrders.Select(x => x.ItemsId), x => x.ItemsId)
            .ToArray();
        
        var result = orders
            .ExceptBy(intersect.Select(x => x.OrderId), x => x.OrderId)
            .ExceptBy(intersect.Select(x => x.ItemsId), x => x.ItemsId)
            .ToArray();
    
        return result;
    }
    
}
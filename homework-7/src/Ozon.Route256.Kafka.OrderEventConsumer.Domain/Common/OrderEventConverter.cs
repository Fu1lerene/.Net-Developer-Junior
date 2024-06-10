using System;
using System.Collections.Generic;
using System.Linq;
using Ozon.Route256.Kafka.OrderEventConsumer.Domain.Entities;
using Ozon.Route256.Kafka.OrderEventConsumer.Domain.Extensions;
using Ozon.Route256.Kafka.OrderEventConsumer.Domain.Order;
using Ozon.Route256.Kafka.OrderEventConsumer.Domain.ValueObjects;
using OrderEventGenerate = Ozon.Route256.Kafka.OrderEventGenerator.Contracts.OrderEvent;

namespace Ozon.Route256.Kafka.OrderEventConsumer.Domain.Common;

public static class OrderEventConverter 
{
    public static OrderEvent ToOrderEventRecord(OrderEventGenerate orderEvent)
    {
        var orderId = new OrderId(orderEvent.OrderId);
        var userId = new UserId(orderEvent.UserId);
        var warehouseId = new WarehouseId(orderEvent.WarehouseId);
        var status = orderEvent.Status.ToStatusRecord();
        var moment = orderEvent.Moment;
        
        var positions = orderEvent.Positions.Select(oep => new OrderEventPosition(
                new ItemId(oep.ItemId),
                oep.Quantity,
                new Money((decimal)(oep.Price.Units + oep.Price.Units / Math.Pow(10, 6)), oep.Price.Currency)))
            .ToArray();

        var result = new OrderEvent(
            orderId,
            userId,
            warehouseId,
            status,
            moment,
            positions);

        return result;
    }

    public static ItemEntity ToItemEntity(OrderEventPosition orderEventPosition, Status status)
    {
        return status switch
        {
            Status.Created => new ItemEntity
            {
                ItemId = orderEventPosition.ItemId.Value,
                UpdatedAt = DateTimeOffset.UtcNow,
                Created = orderEventPosition.Quantity
            },
            Status.Delivered => new ItemEntity
            {
                ItemId = orderEventPosition.ItemId.Value,
                UpdatedAt = DateTimeOffset.UtcNow,
                Delivered = orderEventPosition.Quantity
            },
            Status.Cancelled => new ItemEntity
            {
                ItemId = orderEventPosition.ItemId.Value,
                UpdatedAt = DateTimeOffset.UtcNow,
                Cancelled = orderEventPosition.Quantity
            },
            _ => new ItemEntity()
        };
    }
    
    public static CashAccountEntity ToCashAccountEntity(OrderEventPosition orderEventPosition)
    {
        var result = new CashAccountEntity
        {
            SellerId = GetSellerId(orderEventPosition.ItemId.Value),
            ItemId = GetItemId(orderEventPosition.ItemId.Value),
            Currency = orderEventPosition.Price.Currency,
            Quantity = orderEventPosition.Quantity,
            Price = orderEventPosition.Price.Value
        };

        return result;
    }

    public static IEnumerable<OrderEvent> ToUnicOrderEvents(
        IEnumerable<OrderEvent> orderEvents,
        IEnumerable<OrderEntity> unicOrders)
    {
        return orderEvents.IntersectBy(unicOrders.Select(x => x.OrderId), x => x.OrderId.Value);
    }

    public static OrderEntity ToOrderEntity(OrderEvent orderEvent)
    {
        var itemsId = orderEvent.Positions.Select(oe => oe.ItemId.Value).ToArray();

        var result = new OrderEntity
        {
            OrderId = orderEvent.OrderId.Value,
            Status = orderEvent.Status.ToStatusInt(),
            ItemsId = itemsId
        };

        return result;
    }
    
    private static long GetSellerId(long id)
    {
        var lengthId = 6;
        long sellerId = (long)(id / Math.Pow(10, lengthId));
        
        return sellerId;
    }
    
    private static long GetItemId(long id)
    {
        var lengthId = 6;
        long itemId = (long)(id % Math.Pow(10, lengthId));
        
        return itemId;
    }

    public static long GetFullId(long sellerId, long itemId)
    {
        var lengthId = 6;
        var fullId = (long)(sellerId * Math.Pow(10, lengthId) + itemId);

        return fullId;
    }
}
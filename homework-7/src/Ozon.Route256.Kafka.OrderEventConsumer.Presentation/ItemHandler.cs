using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;
using Confluent.Kafka;
using Microsoft.Extensions.Logging;
using Ozon.Route256.Kafka.OrderEventConsumer.Domain.Common;
using Ozon.Route256.Kafka.OrderEventConsumer.Domain.Services;
using Ozon.Route256.Kafka.OrderEventConsumer.Domain.ValueObjects;
using Ozon.Route256.Kafka.OrderEventConsumer.Infrastructure.Kafka;
using OrderEvent = Ozon.Route256.Kafka.OrderEventGenerator.Contracts.OrderEvent;

namespace Ozon.Route256.Kafka.OrderEventConsumer.Presentation;

public class ItemHandler : IHandler<Ignore, OrderEvent>
{
    private readonly ILogger<ItemHandler> _logger;
    private readonly Random _random = new();
    private readonly IOrderEventService _orderEventService;

    public ItemHandler(ILogger<ItemHandler> logger, IOrderEventService orderEventService)
    {
        _logger = logger;
        _orderEventService = orderEventService;
    }

    public async Task Handle(IReadOnlyCollection<ConsumeResult<Ignore, OrderEvent>> messages, CancellationToken token)
    {
        await Task.Delay(_random.Next(300), token);
        var orderEvents = messages
            .Select(r => OrderEventConverter.ToOrderEventRecord(r.Message.Value));
        var deliveredOrderEvents = orderEvents.Where(oe => oe.Status == Status.Delivered);

        using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
        
        await _orderEventService.AddOrders(orderEvents, token);
        await _orderEventService.AddItems(orderEvents, token);
        await _orderEventService.AddCashAccount(deliveredOrderEvents, token);
        
        scope.Complete();
        
        
        _logger.LogInformation("Handled {Count} messages", messages.Count);
    }
}

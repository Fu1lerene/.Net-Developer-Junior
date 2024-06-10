using System;
using Ozon.Route256.Kafka.OrderEventConsumer.Domain.ValueObjects;

namespace Ozon.Route256.Kafka.OrderEventConsumer.Domain.Entities;

public record OrderEntity
{
    public long OrderId { get; init; }
    
    public int Status { get; init; }

    public long[] ItemsId { get; set; } = Array.Empty<long>();
}
using System;

namespace Ozon.Route256.Kafka.OrderEventConsumer.Domain.Entities;

public record ItemEntity
{
    public long ItemId { get; init; }
    
    public long Created { get; init; }
    
    public long Delivered { get; init; }
    
    public long Cancelled { get; init; }
    
    public DateTimeOffset UpdatedAt { get; init; }
}
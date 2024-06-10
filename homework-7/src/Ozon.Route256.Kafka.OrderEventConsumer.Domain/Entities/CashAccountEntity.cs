namespace Ozon.Route256.Kafka.OrderEventConsumer.Domain.Entities;

public record CashAccountEntity
{
    public long SellerId { get; init; }
    
    public long ItemId { get; init; }

    public string Currency { get; init; } = string.Empty;
    
    public long Quantity { get; init; }
    
    public decimal Price { get; init; }
}
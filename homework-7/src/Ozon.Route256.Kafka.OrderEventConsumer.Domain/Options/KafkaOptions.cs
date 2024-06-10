using System;

namespace Ozon.Route256.Kafka.OrderEventConsumer.Domain.Options;

public class KafkaOptions
{
    public string BootstrapServers { get; init; } = string.Empty;

    public string GroupId { get; init; } = string.Empty;

    public string Topic { get; init; } = string.Empty;
    
    public int ChannelCapacity { get; init; }
    
    public TimeSpan BufferDelay { get; init; }
    
    public int RetryCount { get; init; }
}
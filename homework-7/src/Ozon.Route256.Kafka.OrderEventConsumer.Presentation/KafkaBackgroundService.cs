using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Confluent.Kafka;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Ozon.Route256.Kafka.OrderEventConsumer.Domain.Common;
using Ozon.Route256.Kafka.OrderEventConsumer.Domain.Options;
using Ozon.Route256.Kafka.OrderEventConsumer.Infrastructure.Kafka;
using OrderEvent = Ozon.Route256.Kafka.OrderEventGenerator.Contracts.OrderEvent;

namespace Ozon.Route256.Kafka.OrderEventConsumer.Presentation;

public class KafkaBackgroundService : BackgroundService
{
    private readonly KafkaAsyncConsumer<Ignore, OrderEvent> _consumer;
    private readonly ILogger<KafkaBackgroundService> _logger;

    public KafkaBackgroundService(
        IServiceProvider serviceProvider,
        ILogger<KafkaBackgroundService> logger,
        IOptions<KafkaOptions> options)
    {
        // TODO: KafkaServiceExtensions: services.AddKafkaHandler<TKey, TValue, THandler<TKey, TValue>>(keyDeserializer, valueDeserializer);
        _logger = logger;
        var handler = serviceProvider.GetRequiredService<ItemHandler>();
        _consumer = new KafkaAsyncConsumer<Ignore, OrderEvent>(
            handler,
            options,
            null,
            new CustomJsonSerializer<OrderEvent>(new JsonSerializerOptions
            {
                Converters = { new JsonStringEnumConverter() }
            }),
            serviceProvider.GetRequiredService<ILogger<KafkaAsyncConsumer<Ignore, OrderEvent>>>());
    }

    public override Task StopAsync(CancellationToken cancellationToken)
    {
        _consumer.Dispose();

        return Task.CompletedTask;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            await _consumer.Consume(stoppingToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception occured");
        }
    }
}

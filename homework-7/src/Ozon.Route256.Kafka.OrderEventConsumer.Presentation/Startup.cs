using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Ozon.Route256.Kafka.OrderEventConsumer.Domain.Options;
using Ozon.Route256.Kafka.OrderEventConsumer.Domain.Repositories;
using Ozon.Route256.Kafka.OrderEventConsumer.Domain.Services;
using Ozon.Route256.Kafka.OrderEventConsumer.Infrastructure.Common;
using Ozon.Route256.Kafka.OrderEventConsumer.Infrastructure.Repositories;

namespace Ozon.Route256.Kafka.OrderEventConsumer.Presentation;

public sealed class Startup
{
    private readonly IConfiguration _configuration;

    public Startup(IConfiguration configuration) => _configuration = configuration;

    [Obsolete("Obsolete")]
    public void ConfigureServices(IServiceCollection services)
    {
        services
            .AddLogging();
        
        services.Configure<KafkaOptions>(_configuration.GetSection(nameof(KafkaOptions)));
        
        var connectionString = _configuration["ConnectionString"]!;
        services
            .AddFluentMigrator(
                connectionString,
                typeof(SqlMigration).Assembly);

        
        services.AddSingleton<IItemRepository, ItemRepository>(_ => new ItemRepository(connectionString));
        services.AddSingleton<
            IItemCashAccountingRepository,
            ItemCashAccountingRepository>(_ => new ItemCashAccountingRepository(connectionString));
        services.AddSingleton<IOrderRepository, OrderRepository>(_ => new OrderRepository(connectionString));
        services.AddSingleton<ItemHandler>();
        services.AddHostedService<KafkaBackgroundService>();
        services.AddSingleton<IOrderEventService, OrderEventService>();
        ServiceCollectionExtensions.MapCompositeTypes();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
    }
}

using System;
using System.Reflection;

using FluentMigrator.Runner;
using FluentMigrator.Runner.Processors;

using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using Npgsql.NameTranslation;
using Ozon.Route256.Kafka.OrderEventConsumer.Domain.Entities;

namespace Ozon.Route256.Kafka.OrderEventConsumer.Infrastructure.Common;

public static class ServiceCollectionExtensions
{
    private static readonly INpgsqlNameTranslator Translator = new NpgsqlSnakeCaseNameTranslator();
    
    [Obsolete("Obsolete")]
    public static void MapCompositeTypes()
    {
        var mapper = NpgsqlConnection.GlobalTypeMapper;
        Dapper.DefaultTypeMap.MatchNamesWithUnderscores = true;
        
        mapper.MapComposite<OrderEntity>("orders_type", Translator);
    }
    
    public static IServiceCollection AddFluentMigrator(
        this IServiceCollection services,
        string connectionString,
        Assembly assembly)
    {
        services
            .AddFluentMigratorCore()
            .ConfigureRunner(
                builder => builder
                    .AddPostgres()
                    .ScanIn(assembly).For.Migrations())
            .AddOptions<ProcessorOptions>()
            .Configure(
                options =>
                {
                    options.ProviderSwitches = "Force Quote=false";
                    options.Timeout = TimeSpan.FromMinutes(10);
                    options.ConnectionString = connectionString;
                });
        
        return services;
    }
}

using Homework_3.Configurations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Homework_3;

class Program
{
    static async Task Main(string[] args)
    {
        var services = new ServiceCollection();
        ConfigureServices(services);
        
        await using var serviceProvider = services.BuildServiceProvider();
        
        await serviceProvider.GetService<ConsoleApplication>().RunAsync();
    }
    
    static void ConfigureServices(IServiceCollection services)
    {
        Directory.SetCurrentDirectory("../../../Configurations");
        
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddEnvironmentVariables()
            .Build();
             
        services.Configure<AppSettings>(configuration.GetSection(AppSettings.Position));

        services.AddTransient<ConsoleApplication>().Configure<AppSettings>(configuration);
    }
}
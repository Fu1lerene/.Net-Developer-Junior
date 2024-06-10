using Microsoft.Extensions.DependencyInjection;

namespace Homework4.ProductService.Infrastructure;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddRepositories(this IServiceCollection serviceCollection)
    {
        return serviceCollection.AddSingleton<IProductRepository, ProductRepository>();
    }
}
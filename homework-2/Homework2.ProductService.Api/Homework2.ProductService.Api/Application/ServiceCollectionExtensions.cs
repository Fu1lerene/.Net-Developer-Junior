namespace Homework2.ProductService.Api.Application;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddServices(this IServiceCollection serviceCollection)
    {
        return serviceCollection.AddSingleton<IProductService, ProductService>();
    }
}
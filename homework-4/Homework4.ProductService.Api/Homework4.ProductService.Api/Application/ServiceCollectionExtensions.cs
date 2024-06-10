namespace Homework4.ProductService.Api.Application;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddServices(this IServiceCollection serviceCollection)
    {
        return serviceCollection.AddScoped<IProductService, ProductService>();
    }
}
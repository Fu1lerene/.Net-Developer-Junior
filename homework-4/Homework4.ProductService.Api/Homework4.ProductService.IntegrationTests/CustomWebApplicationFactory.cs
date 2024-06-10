using AutoBogus;
using Homework4.ProductService.Domain;
using Homework4.ProductService.Infrastructure;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Moq;
using Range = Moq.Range;

namespace Homework4.ProductService.IntegrationTests;

public class CustomWebApplicationFactory<TEntryPoint>
    : WebApplicationFactory<TEntryPoint> where TEntryPoint : class
{
    public readonly Mock<IProductRepository> ProductRepositoryFake = new();

    public CustomWebApplicationFactory()
    {
        ProductRepositoryFake
            .Setup(f => f.GetAllProducts())
            .Returns(new List<ProductModel>());
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            services.Replace(ServiceDescriptor.Scoped<IProductRepository>(_ => ProductRepositoryFake.Object));
        });
    }
}
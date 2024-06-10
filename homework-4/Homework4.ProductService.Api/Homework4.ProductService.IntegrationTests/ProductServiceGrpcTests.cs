using AutoBogus;
using FluentAssertions;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Grpc.Net.Client;
using Homework4.ProductService.Api;
using Homework4.ProductService.Api.Extensions;
using Homework4.ProductService.Domain;

namespace Homework4.ProductService.IntegrationTests;

public class ProductServiceGrpcTests : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly CustomWebApplicationFactory<Program> _webApplicationFactory;
    
    public ProductServiceGrpcTests(CustomWebApplicationFactory<Program> webApplicationFactory)
    {
        _webApplicationFactory = webApplicationFactory;
    }
    
    [Fact]
    public async Task GetAllProducts_ShouldReturnAllProducts()
    {
        var numberOfProducts = 20;
        var expectedListOfProducts = new List<ProductModel>();

        for (int i = 0; i < numberOfProducts; i++)
        {
            expectedListOfProducts.Add(AutoFaker.Generate<ProductModel>());
        }

        _webApplicationFactory.ProductRepositoryFake
            .Setup(f => f.GetAllProducts())
            .Returns(expectedListOfProducts);

        var httpClient = _webApplicationFactory.CreateClient();
        var channel = GrpcChannel.ForAddress(httpClient.BaseAddress!, new GrpcChannelOptions()
        {
            HttpClient = httpClient
        });
        var grpcClient = new Api.ProductService.ProductServiceClient(channel);
        
        var response = await grpcClient.GetAllProductsAsync(new Empty());
        
        response.Should().NotBeNull();
        response.Products.Should().NotBeNull().And
            .HaveCount(numberOfProducts).And
            .NotContainNulls();
    }
    
    
    [Fact]
    public async Task GetProductById_ExistingId_ShouldReturnProduct()
    {
        var productId = 1;
        var expectedProduct = new AutoFaker<ProductModel>()
            .RuleFor(f => f.ProductId, productId)
            .Generate();

        _webApplicationFactory.ProductRepositoryFake
            .Setup(f => f.GetProduct(productId))
            .Returns(expectedProduct);
        
        var httpClient = _webApplicationFactory.CreateClient();
        var channel = GrpcChannel.ForAddress(httpClient.BaseAddress!, new GrpcChannelOptions()
        {
            HttpClient = httpClient
        });
        var grpcClient = new Api.ProductService.ProductServiceClient(channel);

        var request = new GetProductRequest
        {
            ProductId = productId
        };
        var response = await grpcClient.GetProductAsync(request);

        response.Should().NotBeNull();

        response.Product.Should().NotBeNull();
        response.Product.ProductId.Should().Be(expectedProduct.ProductId);
        response.Product.ProductType.Should().Be(expectedProduct.Type.GetGrpcProductType());
        response.Product.DateCreation.Should().Be(Timestamp.FromDateTimeOffset(expectedProduct.DateCreation));
        response.Product.Name.Should().Be(expectedProduct.Name);
        response.Product.WarehouseId.Should().Be(expectedProduct.WarehouseId);
        response.Product.Price.Should().Be(expectedProduct.Price);
        response.Product.Weight.Should().Be(expectedProduct.Weight);
    }
    
    [Fact]
    public async Task GetProductById_NotExistingId_ShouldReturnNotFoundException()
    {
        var productId = long.MaxValue;

        _webApplicationFactory.ProductRepositoryFake
            .Setup(f => f.GetProduct(productId))
            .Returns(null as ProductModel);
        
        var httpClient = _webApplicationFactory.CreateClient();
        var channel = GrpcChannel.ForAddress(httpClient.BaseAddress!, new GrpcChannelOptions()
        {
            HttpClient = httpClient
        });
        var grpcClient = new Api.ProductService.ProductServiceClient(channel);

        var request = new GetProductRequest
        {
            ProductId = productId
        };
        
        var response = null as GetProductResponse;
        var statusCode = StatusCode.OK;
        
        try
        {
            response = await grpcClient.GetProductAsync(request);
        }
        catch (RpcException e)
        {
            statusCode = e.StatusCode;
        }
        
        response.Should().BeNull();
        statusCode.Should().Be(StatusCode.NotFound);
    }

    [Fact]
    public async Task SetPrice_ValidProductIdAndPrice_ShouldReturnOk()
    {
        var productId = 1;
        var price = 1d;
        var expectedAnswer = "Цена товара обновлена";

        _webApplicationFactory.ProductRepositoryFake
            .Setup(f => f.GetProduct(productId))
            .Returns(AutoFaker.Generate<ProductModel>());
        
        var httpClient = _webApplicationFactory.CreateClient();
        var channel = GrpcChannel.ForAddress(httpClient.BaseAddress!, new GrpcChannelOptions()
        {
            HttpClient = httpClient
        });
        var grpcClient = new Api.ProductService.ProductServiceClient(channel);

        var request = new SetPriceRequest
        {
            ProductId = productId,
            Price = price
        };

        var response = await grpcClient.SetPriceAsync(request);

        response.Should().NotBeNull();
        response.Response.Should().BeEquivalentTo(expectedAnswer);
    }
    
    [Fact]
    public async Task SetPrice_NotExistProduct_ShouldReturnNotFoundException()
    {
        var productId = long.MaxValue;
        var price = 1d;
        
        var httpClient = _webApplicationFactory.CreateClient();
        var channel = GrpcChannel.ForAddress(httpClient.BaseAddress!, new GrpcChannelOptions()
        {
            HttpClient = httpClient
        });
        var grpcClient = new Api.ProductService.ProductServiceClient(channel);

        var request = new SetPriceRequest
        {
            ProductId = productId,
            Price = price
        };
        
        var response = null as SetPriceResponse;
        var statusCode = StatusCode.OK;
        
        try
        {
            response = await grpcClient.SetPriceAsync(request);
        }
        catch (RpcException e)
        {
            statusCode = e.StatusCode;
        }
        
        response.Should().BeNull();
        statusCode.Should().Be(StatusCode.NotFound);
    }
    
    [Fact]
    public async Task GetFilteredList_ValidFilter_ShouldReturnFilteredList()
    {
        var numberOfProducts = 10;
        var listOfProducts = new List<ProductModel>();

        for (int i = 0; i < numberOfProducts; i++)
        {
            listOfProducts.Add(AutoFaker.Generate<ProductModel>());
        }

        var filter = new AutoFaker<ProductFilter>()
            .RuleFor(f => f.WarehouseId, 1)
            .RuleFor(f => f.Type, ProductType.Default)
            .RuleFor(f => f.StartDate, f => f.Date.Past())
            .RuleFor(f => f.EndDate, f => f.Date.Future())
            .RuleFor(f => f.Page, 1)
            .RuleFor(f => f.PageSize, 1)
            .Generate();

        var expectedListOfProduct = listOfProducts
            .Where(p => p.WarehouseId == filter.WarehouseId)
            .Where(p => p.WarehouseId == filter.WarehouseId)
            .Where(p => p.Type == filter.Type)
            .Where(p => p.DateCreation >= filter.StartDate)
            .Where(p => p.DateCreation <= filter.EndDate);

        var skipPages = (filter.Page - 1) * filter.PageSize;
        expectedListOfProduct = expectedListOfProduct.Skip(skipPages).Take(filter.PageSize);

        _webApplicationFactory.ProductRepositoryFake
            .Setup(f => f.GetFilteredList(filter))
            .Returns(expectedListOfProduct);
        
        var httpClient = _webApplicationFactory.CreateClient();
        var channel = GrpcChannel.ForAddress(httpClient.BaseAddress!, new GrpcChannelOptions()
        {
            HttpClient = httpClient
        });
        var grpcClient = new Api.ProductService.ProductServiceClient(channel);

        var request = new GetFilteredListRequest
        {
            StartDate = filter.StartDate.ToTimestamp(),
            EndDate = filter.EndDate.ToTimestamp(),
            ProductType = filter.Type.GetGrpcProductType(),
            WarehouseId = filter.WarehouseId,
            Page = filter.Page,
            PageSize = filter.PageSize
        };

        var response = await grpcClient.GetFilteredListAsync(request);
        
        response.Should().NotBeNull();
        response.Products.Should().NotBeNull().And
            .HaveCount(expectedListOfProduct.Count()).And
            .NotContainNulls();
    }
    
    [Fact]
    public async Task AddProduct_ValidProduct_ShouldReturnOk()
    {
        var productDto = new AutoFaker<ProductModelDto>()
            .RuleFor(f => f.WarehouseId, 1)
            .RuleFor(f => f.Price, 1)
            .RuleFor(f => f.Weight, 1)
            .Generate();
        
        var product = new ProductModel
        {
            Name = productDto.Name,
            DateCreation = productDto.DateCreation,
            Price = productDto.Price,
            Type = productDto.Type,
            WarehouseId = productDto.WarehouseId,
            Weight = productDto.Weight
        };
        
        var request = new AddProductRequest
        {
            Name = productDto.Name,
            DateCreation = productDto.DateCreation.ToTimestamp(),
            Price = productDto.Price,
            ProductType = productDto.Type.GetGrpcProductType(),
            WarehouseId = productDto.WarehouseId,
            Weight = productDto.Weight
        };

        _webApplicationFactory.ProductRepositoryFake
            .Setup(f => f.AddProduct(product))
            .Returns(product.ProductId);
        
        var expectedAnswer = $"Товар с {product.ProductId + 1} ID успешно добавлен";
        
        var httpClient = _webApplicationFactory.CreateClient();
        var channel = GrpcChannel.ForAddress(httpClient.BaseAddress!, new GrpcChannelOptions()
        {
            HttpClient = httpClient
        });
        var grpcClient = new Api.ProductService.ProductServiceClient(channel);
        
        var response = await grpcClient.AddProductAsync(request);

        response.Should().NotBeNull();
        response.Response.Should().BeEquivalentTo(expectedAnswer);
    }
}
using System.Net;
using System.Net.Http.Json;
using System.Net.Mime;
using System.Text;
using AutoBogus;
using FluentAssertions;
using Homework4.ProductService.Api;
using Homework4.ProductService.Api.Exceptions;
using Homework4.ProductService.Domain;
using Homework4.ProductService.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Moq;
using Newtonsoft.Json;

namespace Homework4.ProductService.IntegrationTests;

public class ProductServiceControllersTests : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly CustomWebApplicationFactory<Program> _webApplicationFactory;

    public ProductServiceControllersTests(CustomWebApplicationFactory<Program> webApplicationFactory)
    {
        _webApplicationFactory = webApplicationFactory;
    }

    [Fact]
    public async Task GetAllProducts_ShouldReturnAllProducts()
    {
        var numberOfProducts = 10;
        var expectedListOfProducts = new List<ProductModel>();

        for (int i = 0; i < numberOfProducts; i++)
        {
            expectedListOfProducts.Add(AutoFaker.Generate<ProductModel>());
        }

        _webApplicationFactory.ProductRepositoryFake
            .Setup(f => f.GetAllProducts())
            .Returns(expectedListOfProducts);

        var client = _webApplicationFactory.CreateClient();

        var response = await client.GetAsync("Product/GetAllProducts");

        var actualListOfProducts = await response.Content.ReadFromJsonAsync<List<ProductModel>>();

        actualListOfProducts.Should().NotBeNull().And
            .BeEquivalentTo(expectedListOfProducts).And
            .NotContainNulls().And
            .HaveCount(numberOfProducts);

        response.Should().BeSuccessful().And
            .HaveStatusCode(HttpStatusCode.OK).And
            .Match(r => r.Version == HttpVersion.Version11);
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

        var client = _webApplicationFactory.CreateClient();
        var response = await client.GetAsync($"Product/GetProduct/{productId}");

        var actualProduct = await response.Content.ReadFromJsonAsync<ProductModel>();

        response.Should().BeSuccessful().And
            .HaveStatusCode(HttpStatusCode.OK).And
            .Match(r => r.Version == HttpVersion.Version11);

        actualProduct.Should().NotBeNull().And
            .BeEquivalentTo(expectedProduct);
    }

    [Fact]
    public async Task GetProductById_NotExistingId_ShouldReturnNotFoundError()
    {
        var productId = long.MaxValue;
        var expectedAnswer = $"Товар с {productId} ID не найден";
        
        _webApplicationFactory.ProductRepositoryFake
            .Setup(f => f.GetProduct(productId))
            .Throws(new ProductNotFoundException(productId));
        
        var client = _webApplicationFactory.CreateClient();
        try
        {
            await client.GetAsync($"Product/GetProduct/{productId}");
        }
        catch (ProductNotFoundException e)
        {
            e.Message.Should().BeEquivalentTo(expectedAnswer);
        }
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(long.MinValue)]
    public async Task GetProductById_NotPositiveId_ShouldReturnBedRequestError(long productId)
    {
        var client = _webApplicationFactory.CreateClient();
        var response = await client.GetAsync($"Product/GetProduct/{productId}");

        response.Should().HaveError().And
            .HaveStatusCode(HttpStatusCode.BadRequest).And
            .Match(r => r.Version == HttpVersion.Version11);
    }

    [Fact]
    public async Task SetPrice_ValidProductIdAndPrice_ShouldReturnOk()
    {
        var productId = 1;
        var price = 1d;
        var setPriceModel = new SetPriceModel
        {
            ProductId = productId,
            Price = price
        };

        var client = _webApplicationFactory.CreateClient();

        var response = await client.PutAsJsonAsync("/Product/SetPrice", setPriceModel);

        response.Should().BeSuccessful().And
            .HaveStatusCode(HttpStatusCode.OK).And
            .Match(r => r.Version == HttpVersion.Version11);
    }

    [Fact]
    public async Task SetPrice_NotExistingProduct_ShouldReturnNotFoundError()
    {
        var productId = long.MaxValue;
        var price = 1d;
        var setPriceModel = new SetPriceModel
        {
            ProductId = productId,
            Price = price
        };
        var expectedAnswer = $"Товар с {productId} ID не найден";

        _webApplicationFactory.ProductRepositoryFake
            .Setup(f => f.SetPrice(setPriceModel))
            .Throws(new ProductNotFoundException(productId));
        
        var client = _webApplicationFactory.CreateClient();
        try
        {
            await client.PutAsJsonAsync("/Product/SetPrice", setPriceModel);
        }
        catch (ProductNotFoundException e)
        {
            e.Message.Should().BeEquivalentTo(expectedAnswer);
        }
    }

    [Theory]
    [InlineData(0, 0)]
    [InlineData(-1, -1)]
    [InlineData(1, -1)]
    [InlineData(-1, 1)]
    public async Task SetPrice_NotValidPriceOrProductId_ShouldReturnBadRequestError(
        long productId, double price)
    {
        var setPriceModel = new SetPriceModel
        {
            ProductId = productId,
            Price = price
        };

        var client = _webApplicationFactory.CreateClient();

        var response = await client.PutAsJsonAsync("/Product/SetPrice", setPriceModel);

        response.Should().HaveError().And
            .HaveStatusCode(HttpStatusCode.BadRequest).And
            .Match(r => r.Version == HttpVersion.Version11);
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

        var client = _webApplicationFactory.CreateClient();

        var request = new HttpRequestMessage
        {
            Method = HttpMethod.Get,
            RequestUri = new Uri("http://localhost:5080/Product/GetFilteredList"),
            Content = new StringContent(
                JsonConvert.SerializeObject(filter),
                Encoding.UTF8,
                MediaTypeNames.Application.Json)
        };

        _webApplicationFactory.ProductRepositoryFake
            .Setup(f => f.GetFilteredList(filter))
            .Returns(expectedListOfProduct);

        var response = await client.SendAsync(request);
        var actualListOfProducts = await response.Content.ReadFromJsonAsync<List<ProductModel>>();

        actualListOfProducts.Should().NotBeNull().And
            .BeEquivalentTo(expectedListOfProduct).And
            .NotContainNulls();

        response.Should().BeSuccessful().And
            .HaveStatusCode(HttpStatusCode.OK).And
            .Match(r => r.Version == HttpVersion.Version11);
    }

    [Fact]
    public async Task GetFilteredList_NotValidFilter_ShouldReturnBedRequestError()
    {
        var filter = new AutoFaker<ProductFilter>()
            .RuleFor(f => f.Page, -1)
            .Generate();

        var client = _webApplicationFactory.CreateClient();

        var request = new HttpRequestMessage
        {
            Method = HttpMethod.Get,
            RequestUri = new Uri("http://localhost:5080/Product/GetFilteredList"),
            Content = new StringContent(
                JsonConvert.SerializeObject(filter),
                Encoding.UTF8,
                MediaTypeNames.Application.Json)
        };

        _webApplicationFactory.ProductRepositoryFake
            .Setup(f => f.GetFilteredList(filter))
            .Returns(null as List<ProductModel>);

        var response = await client.SendAsync(request);

        response.Should().HaveError().And
            .HaveStatusCode(HttpStatusCode.BadRequest).And
            .Match(r => r.Version == HttpVersion.Version11);
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
        
        _webApplicationFactory.ProductRepositoryFake
            .Setup(f => f.AddProduct(product))
            .Returns(product.ProductId);
        
        var expectedAnswer = $"Продукт с {product.ProductId + 1} ID успешно добавлен";
        
        var client = _webApplicationFactory.CreateClient();
        var response = await client.PostAsJsonAsync("Product/AddProduct", productDto);

        var actualAnswer = await response.Content.ReadAsStringAsync();

        actualAnswer.Should().NotBeNull().And
            .BeEquivalentTo(expectedAnswer);
        
        response.Should().BeSuccessful().And
            .HaveStatusCode(HttpStatusCode.OK).And
            .Match(r => r.Version == HttpVersion.Version11);
    }
    
    [Fact]
    public async Task AddProduct_NotValidProduct_ShouldReturnBedRequestError()
    {
        var productDto = new AutoFaker<ProductModelDto>()
            .RuleFor(f => f.Price, -1)
            .Generate();
        
        var client = _webApplicationFactory.CreateClient();
        var response = await client.PostAsJsonAsync("Product/AddProduct", productDto);
        
        response.Should().HaveError().And
            .HaveStatusCode(HttpStatusCode.BadRequest).And
            .Match(r => r.Version == HttpVersion.Version11);
    }
}
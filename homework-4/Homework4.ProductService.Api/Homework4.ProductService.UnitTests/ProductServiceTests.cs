using AutoBogus;
using FluentAssertions;
using Homework4.ProductService.Api.Application;
using Homework4.ProductService.Api.Exceptions;
using Homework4.ProductService.Domain;
using Homework4.ProductService.Infrastructure;
using Moq;

namespace Homework4.ProductService.UnitTests;

public class ProductServiceTests
{
    private readonly Mock<IProductRepository> _productRepositoryFake = new();
    private readonly IProductService _productService;

    public ProductServiceTests()
    {
        _productService = new Api.Application.ProductService(_productRepositoryFake.Object);
    }
    
    [Fact]
    public void GetProductById_NotNullProduct_ShouldReturnProduct()
    {
        var productId = 1;
        var expectedProduct = AutoFaker.Generate<ProductModel>();

        _productRepositoryFake
            .Setup(f => f.GetProduct(productId))
            .Returns(expectedProduct);

        var actualProduct = _productService.GetProduct(productId);

        actualProduct.Should().NotBeNull().And.BeEquivalentTo(expectedProduct);
        _productRepositoryFake
            .Verify(f => f.GetProduct(It.IsAny<long>()), Times.Once);
    }
    
    [Fact]
    public void GetProductById_NullProduct_ShouldReturnNull()
    {
        ProductModel? expectedProduct = null;

        _productRepositoryFake
            .Setup(f => f.GetProduct(It.IsAny<long>()))
            .Returns(null as ProductModel);

        var actualProduct = _productService.GetProduct(It.IsAny<long>());

        actualProduct.Should().BeNull().And.BeEquivalentTo(expectedProduct);
        _productRepositoryFake.Verify(f => f.GetProduct(It.IsAny<long>()), Times.Once);
    }
    
    [Fact]
    public void GetProductById_NotExistingProduct_ShouldReturnProductNotFoundException()
    {
        var productId = long.MaxValue;
        var expectedAnswer = $"Товар с {productId} ID не найден";
        
        _productRepositoryFake
            .Setup(f => f.GetProduct(productId))
            .Throws(new Exception());

        try
        {
            _productService.GetProduct(productId);
        }
        catch (ProductNotFoundException e)
        {
            e.Message.Should().BeEquivalentTo(expectedAnswer);
                    
            _productRepositoryFake
                .Verify(f => f.GetProduct(It.IsAny<long>()), Times.Once);
        }
    }

    [Fact]
    public void GetAllProducts_NotEmptyList_ShouldReturnAllProducts()
    {
        var expectedListOfProducts = new List<ProductModel>();
        var numberOfProducts = 10;

        for (int i = 0; i < numberOfProducts; i++)
        {
            expectedListOfProducts.Add(AutoFaker.Generate<ProductModel>());
        }

        _productRepositoryFake
            .Setup(f => f.GetAllProducts())
            .Returns(expectedListOfProducts);
        
        var actualListOfProducts = _productService.GetAllProducts();

        actualListOfProducts.Should().NotBeNull().And
            .BeEquivalentTo(expectedListOfProducts).And
            .NotBeEmpty().And
            .HaveCount(numberOfProducts).And
            .NotContainNulls();
        
        _productRepositoryFake.Verify(f => f.GetAllProducts(), Times.Once);
    }
    
    [Theory]
    [InlineData(1, 1.0)]
    [InlineData(long.MinValue, double.MinValue)]
    [InlineData(long.MaxValue, double.MaxValue)]
    public void SetPrice_NotNullProduct_ShouldSetNewPrice(
        long productId, double price)
    {
        var setPriceModel = new SetPriceModel
        {
            ProductId = productId,
            Price = price
        };
        
        var expectedProduct = new AutoFaker<ProductModel>()
            .RuleFor(p => p.ProductId, productId)
            .RuleFor(p => p.Price, price)
            .Generate();
        
        _productRepositoryFake
            .Setup(f => f.SetPrice(setPriceModel))
            .Verifiable();
        
        _productRepositoryFake
            .Setup(f => f.GetProduct(productId))
            .Returns(expectedProduct);
        
        _productService.SetPrice(setPriceModel);
        
        var actualProduct = _productService.GetProduct(productId);

        actualProduct.Should().NotBeNull().And.BeEquivalentTo(expectedProduct);
        _productRepositoryFake
            .Verify(f => f.SetPrice(setPriceModel), Times.Once);
    }
    
    [Theory]
    [InlineData(0, 1.0)]
    [InlineData(long.MinValue, double.MinValue)]
    public void SetPrice_NotExistingProduct_ShouldReturnProductNotFoundException(
        long productId, double price)
    {
        var setPriceModel = new SetPriceModel
        {
            ProductId = productId,
            Price = price
        };
        var expectedAnswer = $"Товар с {productId} ID не найден";

        _productRepositoryFake
            .Setup(f => f.SetPrice(setPriceModel))
            .Throws(new ProductNotFoundException(productId));

        try
        {
            _productService.SetPrice(setPriceModel);
        }
        catch (ProductNotFoundException e)
        {
            e.Message.Should().BeEquivalentTo(expectedAnswer);
            _productRepositoryFake
                .Verify(f => f.SetPrice(setPriceModel), Times.Once);
        }
    }

    [Fact]
    public void GetFilteredListOfProducts_NotNullFilter_ShouldReturnFilteredListOfProducts()
    {
        var filter = AutoFaker.Generate<ProductFilter>();
        var listOfProducts = new List<ProductModel>();
        var numberOfProducts = 10;
        
        for (int i = 0; i < numberOfProducts; i++)
        {
            listOfProducts.Add(AutoFaker.Generate<ProductModel>());
        }

        var expectedListOfProducts = listOfProducts
            .Where(p => p.WarehouseId == filter.WarehouseId)
            .Where(p => p.Type == filter.Type)
            .Where(p => p.DateCreation >= filter.StartDate)
            .Where(p => p.DateCreation <= filter.EndDate);

        _productRepositoryFake
            .Setup(f => f.GetFilteredList(filter))
            .Returns(expectedListOfProducts);
        
        var actualListOfProducts = _productService.GetFilteredList(filter);

        actualListOfProducts.Should().NotBeNull().And
            .BeEquivalentTo(expectedListOfProducts).And
            .NotContainNulls();
        
        _productRepositoryFake
            .Verify(f => f.GetFilteredList(It.IsAny<ProductFilter>()), Times.Once);
    }
    
    [Fact]
    public void GetFilteredListOfProducts_NullFilter_ShouldReturnAllProducts()
    {
        ProductFilter? filter = null;
        var expectedListOfProducts = new List<ProductModel>();
        var numberOfProducts = 10;
        
        for (int i = 0; i < numberOfProducts; i++)
        {
            expectedListOfProducts.Add(AutoFaker.Generate<ProductModel>());
        }

        _productRepositoryFake
            .Setup(f => f.GetFilteredList(filter))
            .Returns(expectedListOfProducts);
        
        var actualListOfProducts = _productService.GetFilteredList(filter);

        actualListOfProducts.Should().NotBeNull().And
            .BeEquivalentTo(expectedListOfProducts).And
            .NotBeEmpty().And
            .HaveCount(numberOfProducts).And
            .NotContainNulls();
        
        _productRepositoryFake
            .Verify(f => f.GetFilteredList(It.IsAny<ProductFilter>()), Times.Once);
    }

    [Fact]
    public void AddProduct_NotNullProductModel_ShouldReturnProductId()
    {
        var expectedProductId = 2;
        var newProductDto = AutoFaker.Generate<ProductModelDto>();
        var expectedProduct = new ProductModel
        {
            Name = newProductDto.Name,
            DateCreation = newProductDto.DateCreation,
            Price = newProductDto.Price,
            Type = newProductDto.Type,
            WarehouseId = newProductDto.WarehouseId,
            Weight = newProductDto.Weight
        };

        _productRepositoryFake
            .Setup(f => f.AddProduct(expectedProduct))
            .Returns(expectedProduct.ProductId);
        
        var actualProductId = _productService.AddProduct(newProductDto);

        actualProductId.Should().Be(expectedProductId);
        _productRepositoryFake
            .Verify(f => f.AddProduct(It.IsAny<ProductModel>()), Times.Once);
    }
}
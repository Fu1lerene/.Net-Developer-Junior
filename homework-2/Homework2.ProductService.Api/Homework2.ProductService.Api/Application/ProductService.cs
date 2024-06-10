using Homework2.ProductService.Domain;
using Homework2.ProductService.Infrastructure;

namespace Homework2.ProductService.Api.Application;

public class ProductService : IProductService
{
    private readonly IProductRepository _productRepository;

    public ProductService(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public void AddProduct(ProductModelDto productDto)
    {
        var product = new ProductModel
        {
            Name = productDto.Name,
            DateCreation = productDto.DateCreation,
            Price = productDto.Price,
            Type = productDto.Type,
            WarehouseId = productDto.WarehouseId,
            Weight = productDto.Weight
        };
        _productRepository.AddProduct(product);
    }

    public IEnumerable<ProductModel> GetAllProducts()
    {
        return _productRepository.GetAllProducts();
    }

    public void SetPrice(long productId, double newPrice)
    {
        _productRepository.SetPrice(productId, newPrice);
    }

    public ProductModel GetProduct(long productId)
    {
        return _productRepository.GetProduct(productId);
    }

    public IEnumerable<ProductModel> GetFilteredList(ProductFilter productFilter)
    {
        return _productRepository.GetFilteredList(productFilter);
    }
}
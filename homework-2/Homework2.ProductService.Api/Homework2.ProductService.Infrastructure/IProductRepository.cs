using Homework2.ProductService.Domain;

namespace Homework2.ProductService.Infrastructure;

public interface IProductRepository
{
    void AddProduct(ProductModel product);
    IEnumerable<ProductModel> GetFilteredList(ProductFilter productFilter);
    IEnumerable<ProductModel> GetAllProducts();
    ProductModel GetProduct(long productId);
    void SetPrice(long productId, double newPrice);
}
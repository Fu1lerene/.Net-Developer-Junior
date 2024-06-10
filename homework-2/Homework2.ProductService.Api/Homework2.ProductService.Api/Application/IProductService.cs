using Homework2.ProductService.Domain;

namespace Homework2.ProductService.Api.Application;

public interface IProductService
{
    void AddProduct(ProductModelDto product);
    IEnumerable<ProductModel> GetAllProducts();
    void SetPrice(long productId, double newPrice);
    ProductModel GetProduct(long productId);
    IEnumerable<ProductModel> GetFilteredList(ProductFilter productFilter);

}
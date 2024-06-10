using Homework4.ProductService.Domain;

namespace Homework4.ProductService.Infrastructure;

public interface IProductRepository
{
    long AddProduct(ProductModel product);
    IEnumerable<ProductModel> GetFilteredList(ProductFilter productFilter);
    IEnumerable<ProductModel> GetAllProducts();
    ProductModel? GetProduct(long productId);
    void SetPrice(SetPriceModel setPriceModel);
}
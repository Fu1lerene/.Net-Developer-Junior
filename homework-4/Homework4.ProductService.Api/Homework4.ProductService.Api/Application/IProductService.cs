using System.Collections.Generic;
using Homework4.ProductService.Domain;

namespace Homework4.ProductService.Api.Application;

public interface IProductService
{
    long AddProduct(ProductModelDto product);
    IEnumerable<ProductModel> GetAllProducts();
    void SetPrice(SetPriceModel setPriceModel);
    ProductModel? GetProduct(long productId);
    IEnumerable<ProductModel> GetFilteredList(ProductFilter productFilter);

}
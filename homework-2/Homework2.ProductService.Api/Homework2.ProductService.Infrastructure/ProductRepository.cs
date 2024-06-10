using System.Collections.Concurrent;
using Homework2.ProductService.Domain;

namespace Homework2.ProductService.Infrastructure;

public class ProductRepository : IProductRepository
{
    private readonly ConcurrentDictionary<long, ProductModel> _store;

    public ProductRepository()
    {
        _store = new ();
    }
    
    public void AddProduct(ProductModel product)
    {
        _store.TryAdd(product.ProductId, product);
    }

    public IEnumerable<ProductModel> GetFilteredList(ProductFilter productFilter)
    {
        var products = (IEnumerable<ProductModel>)_store.Values;
        products = products.Where(p => p.WarehouseId == productFilter.WarehouseId);
        products = products.Where(p => p.Type == productFilter.Type);
        products = products.Where(p => p.DateCreation >= productFilter.StartDate);
        products = products.Where(p => p.DateCreation <= productFilter.EndDate);

        var skipPages = (productFilter.Page - 1) * productFilter.PageSize;
        products = products.Skip(skipPages).Take(productFilter.PageSize);
        
        return products;
    }

    public IEnumerable<ProductModel> GetAllProducts()
    {
        return _store.Values;
    }

    public ProductModel GetProduct(long productId)
    {
        if (_store.TryGetValue(productId, out var product))
            return product;

        return null;
    }

    public void SetPrice(long productId, double newPrice)
    {
        _store[productId].Price = newPrice;
    }
}

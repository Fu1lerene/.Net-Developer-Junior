using System.Collections.Concurrent;
using Homework4.ProductService.Domain;

namespace Homework4.ProductService.Infrastructure;

public class ProductRepository : IProductRepository
{
    private readonly ConcurrentDictionary<long, ProductModel> _store;

    public ProductRepository()
    {
        _store = new ();
    }
    
    public long AddProduct(ProductModel product)
    {
        _store.TryAdd(product.ProductId, product);
        return product.ProductId;
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
        {
            return product;
        }

        throw new Exception();
    }

    public void SetPrice(SetPriceModel setPriceModel)
    {
        if (_store.TryGetValue(setPriceModel.ProductId, out var product))
        {
            _store[setPriceModel.ProductId].Price = setPriceModel.Price;
            return;
        }

        throw new Exception();
    }
}

using DataAccess.Repositories.Interfaces;
using Models;

namespace DataAccess.Repositories;

public class ProductRepository : IProductRepository
{
    public int NumberProducts { get; init; }
    private static readonly Dictionary<int, ProductModel> Products = new();

    public ProductRepository(int numberProducts)
    {
        NumberProducts = numberProducts;
        GenerateProducts(numberProducts);
    }

    private void GenerateProducts(int numberProducts)
    {
        for (int i = 1; i <= numberProducts; i++)
        {
            Products.Add(i, new ProductModel
            {
                Id = i,
                Price = Random.Shared.Next(100, 100000) / 100m
            });
        }
    }
    
    public ProductModel Get(int productId)
    {
        if (Products.ContainsKey(productId))
        {
            return Products[productId];
        }
        return new ProductModel();
    }
}
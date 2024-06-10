using Models;

namespace DataAccess.Repositories.Interfaces;

public interface IProductRepository
{
    public int NumberProducts { get; init; }
    public ProductModel Get(int productId);
}
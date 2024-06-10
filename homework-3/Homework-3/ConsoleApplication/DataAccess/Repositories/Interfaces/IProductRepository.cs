using Models;

namespace Homework_3.DataAccess.Repositories.Interfaces;

public interface IProductRepository
{
    public int NumberProducts { get; init; }
    public ProductModel Get(int productId);
}
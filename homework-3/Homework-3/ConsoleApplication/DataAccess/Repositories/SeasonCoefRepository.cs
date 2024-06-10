using Homework_3.DataAccess.Repositories.Interfaces;
using Models;

namespace Homework_3.DataAccess.Repositories;

public class SeasonCoefRepository : ISeasonCoefRepository
{
    private readonly List<SeasonCoefModel> _seasonCoefRepository = new();

    public SeasonCoefRepository(IProductRepository products)
    {
        GenerateCoefs(products);
    }
    
    private void GenerateCoefs(IProductRepository products)
    {
        for (int i = 1; i <= products.NumberProducts; i++)
        {
            for (int month = 1; month <= 12; month++)
            {
                _seasonCoefRepository.Add(new SeasonCoefModel
                {
                    ProductId = products.Get(i).Id,
                    Month = month,
                    Coef = Random.Shared.Next(0, 30) / 10f
                });
            }
        }
    }

    public List<SeasonCoefModel> Get(long productId)
    {
        return _seasonCoefRepository.Where(x => x.ProductId == productId).ToList();
    }
}
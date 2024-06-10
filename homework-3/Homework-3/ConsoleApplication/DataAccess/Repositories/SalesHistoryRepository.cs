using Homework_3.DataAccess.Repositories.Interfaces;
using Models;

namespace Homework_3.DataAccess.Repositories;

public class SalesHistoryRepository : ISalesHistoryRepository
{
    private static List<SalesHistoryModel> _salesHistoryRepository = new ();

    public SalesHistoryRepository(IProductRepository productRepository, long numberRecords)
    {
        GenerateSalesHistory(productRepository, numberRecords);
    }

    private void GenerateSalesHistory(IProductRepository products, long numberRecords)
    {
        for (int i = 1; i <= numberRecords; i++)
        {
            _salesHistoryRepository.Add(new SalesHistoryModel
            {
                ProductId = products.Get(Random.Shared.Next(1, products.NumberProducts + 1)).Id,
                Date = DateTimeOffset.Now.AddDays(Random.Shared.Next(1, 365) * -1),
                Sales = Random.Shared.Next(0, 20),
                Stock = Random.Shared.Next(20, 40)
            });
        }

        _salesHistoryRepository = _salesHistoryRepository
            .OrderBy(x => x.ProductId)
            .ThenBy(x => x.Date)
            .ToList();
    }
    
    public List<SalesHistoryModel> Get(long productId)
    {
        return _salesHistoryRepository.Where(x => x.ProductId == productId).ToList();
    }

    public List<SalesHistoryModel> GetAll()
    {
        return _salesHistoryRepository;
    }
}
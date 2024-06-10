using DataAccess.Repositories.Interfaces;

namespace Domain.Services;

public class CalculateAds : ICalculateAds
{
    private readonly ISalesHistoryRepository _historyRepository;

    public CalculateAds(ISalesHistoryRepository salesHistoryRepository)
    {
        _historyRepository = salesHistoryRepository;
    }
    
    public float Ads(int productId)
    {
        var salesHistory = _historyRepository.Get(productId);
        
        if (salesHistory.Count == 0)
        {
            throw new ProductNotFoundException($"{productId} ID not found");
        }

        var sumSales = (float)salesHistory
            .Where(x => x.Sales != 0)
            .Sum(x => x.Sales);
        
        return sumSales / salesHistory.Count;
    }
}


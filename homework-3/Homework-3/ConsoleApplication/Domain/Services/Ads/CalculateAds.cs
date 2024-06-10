using Homework_3.DataAccess.Repositories.Interfaces;
using Homework_3.Domain.Exceptions;

namespace Homework_3.Domain.Services.Ads;

public class CalculateAds : ICalculateAds
{
    private readonly ISalesHistoryRepository _salesHistoryRepository;

    public CalculateAds(ISalesHistoryRepository salesHistoryRepository)
    {
        _salesHistoryRepository = salesHistoryRepository;
    }
    
    public float Ads(long productId)
    {
        var salesHistory = _salesHistoryRepository.Get(productId);
        
        if (salesHistory.Count == 0)
        {
            throw new ProductNotFoundException(productId);
        }

        var sumSales = (float)salesHistory
            .Where(x => x.Sales != 0)
            .Sum(x => x.Sales);
        
        return sumSales / salesHistory.Count;
    }
}
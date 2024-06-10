using DataAccess.Repositories.Interfaces;

namespace Domain.Services.SalesPrediction;

public class CalculateSalesPrediction : ICalculateSalesPrediction
{
    private readonly ICalculateAds _calculateAds;
    private readonly ISeasonCoefRepository _coefRepository;
    
    public CalculateSalesPrediction(ISalesHistoryRepository salesHistoryRepository, ISeasonCoefRepository seasonCoefRepository)
    {
        _calculateAds = new CalculateAds(salesHistoryRepository);
        _coefRepository = seasonCoefRepository;
    }
    
    public int SalesPrediction(int productId, int numberDays)
    {
        var countDays = 0;
        var seasonCoefs = new List<float>();
        var coefs = _coefRepository.Get(productId);
        
        if (coefs.Count == 0)
        {
            throw new ProductNotFoundException($"{productId} ID not found");
        }
        
        var currentMonth = DateTimeOffset.Now.Month;
        
        for (int i = 0; i <= numberDays; i++)
        {
            var month = DateTimeOffset.Now.AddDays(i).Month;
            
            if (currentMonth == month)
            {
                countDays++;
            }
            else
            {
                seasonCoefs.Add(countDays * coefs.Find(x => x.Month == currentMonth)!.Coef);
                countDays = 0;
                currentMonth++;
            }
            currentMonth %= 13;
            currentMonth = currentMonth == 0 ? 1 : currentMonth;
        }
        seasonCoefs.Add(countDays * coefs.Find(x => x.Month == currentMonth)!.Coef);
        
        var ads = _calculateAds.Ads(productId);
        var seasonCoefMultiplier = seasonCoefs.Sum();
        
        return (int)Math.Ceiling(ads * seasonCoefMultiplier);
    }
}
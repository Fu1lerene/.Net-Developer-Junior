using DataAccess.Repositories.Interfaces;
using Domain.Services.SalesPrediction;

namespace Domain.Services.Demand;

public class CalculateDemand : ICalculateDemand
{
    private readonly ICalculateSalesPrediction _calculateSalesPrediction;
    private readonly ISalesHistoryRepository _historyRepository;
    public CalculateDemand(ISalesHistoryRepository salesHistoryRepository, ISeasonCoefRepository seasonCoefRepository)
    {
        _calculateSalesPrediction = new CalculateSalesPrediction(salesHistoryRepository, seasonCoefRepository);
        _historyRepository = salesHistoryRepository;
    }
    
    public int Demand(int productId, int numberDays)
    {
        var salesHistoryList = _historyRepository.Get(productId);
        
        if (salesHistoryList.Count == 0)
        {
            throw new ProductNotFoundException($"{productId} ID not found");
        }
        
        var saleHistory = salesHistoryList.OrderBy(x => x.Date).Last();
        var salesPrediction = _calculateSalesPrediction.SalesPrediction(productId, numberDays);
        var quantityProducts = saleHistory.Stock - saleHistory.Sales;
        
        return  salesPrediction - quantityProducts;
    }
}
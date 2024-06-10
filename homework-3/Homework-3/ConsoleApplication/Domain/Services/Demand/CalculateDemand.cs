using Homework_3.DataAccess.Repositories.Interfaces;
using Homework_3.Domain.Exceptions;
using Homework_3.Domain.Services.SalesPrediction;

namespace Homework_3.Domain.Services.Demand;

public class CalculateDemand : ICalculateDemand
{
    private readonly ICalculateSalesPrediction _calculateSalesPrediction;
    private readonly ISalesHistoryRepository _historyRepository;
    public CalculateDemand(ISalesHistoryRepository salesHistoryRepository, ISeasonCoefRepository seasonCoefRepository)
    {
        _calculateSalesPrediction = new CalculateSalesPrediction(salesHistoryRepository, seasonCoefRepository);
        _historyRepository = salesHistoryRepository;
    }
    
    public int Demand(long productId, int numberDays)
    {
        var salesHistoryList = _historyRepository.Get(productId);
        
        if (salesHistoryList.Count == 0)
        {
            throw new ProductNotFoundException(productId);
        }
        
        var saleHistory = salesHistoryList.OrderBy(x => x.Date).Last();
        var salesPrediction = _calculateSalesPrediction.SalesPrediction(productId, numberDays);
        var quantityProducts = saleHistory.Stock - saleHistory.Sales;
        
        return  salesPrediction - quantityProducts;
    }
}
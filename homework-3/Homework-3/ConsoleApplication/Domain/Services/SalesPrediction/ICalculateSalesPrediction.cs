namespace Homework_3.Domain.Services.SalesPrediction;

public interface ICalculateSalesPrediction
{
    public int SalesPrediction(long productId, int numberDays);
}
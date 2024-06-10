namespace Domain.Services.SalesPrediction;

public interface ICalculateSalesPrediction
{
    public int SalesPrediction(int productId, int numberDays);
}
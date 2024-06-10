namespace Domain.Services.Demand;

public interface ICalculateDemand
{
    public int Demand(int productId, int numberDays);
}
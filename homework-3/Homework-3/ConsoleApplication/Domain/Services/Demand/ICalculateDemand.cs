namespace Homework_3.Domain.Services.Demand;

public interface ICalculateDemand
{
    public int Demand(long productId, int numberDays);
}
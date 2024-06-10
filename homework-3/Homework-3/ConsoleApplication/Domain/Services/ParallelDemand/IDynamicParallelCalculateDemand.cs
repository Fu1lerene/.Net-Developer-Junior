namespace Homework_3.Domain.Services.ParallelDemand;

public interface IDynamicParallelCalculateDemand
{
    Task CalculateAndWriteToTxtAsync();
}
namespace HomeworkApp.Dal.Repositories.Interfaces;

public interface IRateLimiterRepository
{
    Task<long> GetCountRequests(string clientName);
}
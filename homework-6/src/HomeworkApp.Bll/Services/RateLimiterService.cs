using HomeworkApp.Bll.Services.Interfaces;
using HomeworkApp.Dal.Repositories.Interfaces;

namespace HomeworkApp.Bll.Services;

public class RateLimiterService : IRateLimiterService
{
    private readonly IRateLimiterRepository _rateLimiterRepository;
    
    public RateLimiterService(IRateLimiterRepository rateLimiterRepository)
    {
        _rateLimiterRepository = rateLimiterRepository;
    }
    
    public async Task ThrowIfTooManyRequests(string clientName, CancellationToken token)
    {
        token.ThrowIfCancellationRequested();

        var countRequests = await _rateLimiterRepository.GetCountRequests(clientName);
        
        if (countRequests > 0)
        {
            return;
        }

        throw new InvalidOperationException("Too many requests");
    }
}
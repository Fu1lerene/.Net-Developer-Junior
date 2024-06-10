namespace HomeworkApp.Bll.Services.Interfaces;

public interface IRateLimiterService
{
    Task ThrowIfTooManyRequests(string clientName, CancellationToken token);
}
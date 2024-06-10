using HomeworkApp.Bll.Services;
using HomeworkApp.Bll.Services.Interfaces;
using HomeworkApp.Dal.Repositories.Interfaces;
using Moq;

namespace HomeworkApp.UnitTests;

public class RateLimiterServiceTests
{
    private readonly IRateLimiterService _rateLimiter;
    private readonly Mock<IRateLimiterRepository> _rateLimiterRepositoryFake = new();

    public RateLimiterServiceTests()
    {
        _rateLimiter = new RateLimiterService(_rateLimiterRepositoryFake.Object);
    }
    
    [Fact]
    public async Task RateLimiter_limitNotExceeded_success()
    {
        // Arrange
        var clientName = "123";
        
        _rateLimiterRepositoryFake
            .Setup(f => f.GetCountRequests(clientName))
            .ReturnsAsync(Random.Shared.Next(1, 100));

        try
        {
            await _rateLimiter.ThrowIfTooManyRequests(clientName, default);
        }
        catch (InvalidOperationException e)
        {
            Assert.Fail("fail");
        }
    }
    
    [Fact]
    public async Task RateLimiter_limitExceeded_ThrowException()
    {
        // Arrange
        var clientName = "1234";
        
        _rateLimiterRepositoryFake
            .Setup(f => f.GetCountRequests(clientName))
            .ReturnsAsync(0);
        
        // Asserts
        await Assert.ThrowsAsync<InvalidOperationException>
            (async () => await _rateLimiter.ThrowIfTooManyRequests(clientName, default));
    }
}
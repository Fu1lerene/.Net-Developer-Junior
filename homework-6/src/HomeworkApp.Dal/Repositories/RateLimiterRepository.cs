using HomeworkApp.Dal.Repositories.Interfaces;
using HomeworkApp.Dal.Settings;
using Microsoft.Extensions.Options;
using StackExchange.Redis;

namespace HomeworkApp.Dal.Repositories;

public class RateLimiterRepository : RedisRepository, IRateLimiterRepository
{
    protected override TimeSpan KeyTtl => TimeSpan.FromMinutes(1);
    
    protected override string KeyPrefix => "rate-limit";

    private readonly IOptions<DalOptions> _settings;

    public RateLimiterRepository(IOptions<DalOptions> dalSettings) : base(dalSettings.Value)
    {
        _settings = dalSettings;
    }

    public async Task<long> GetCountRequests(string clientName)
    {
        var db = await GetConnection();
        var key = GetKey(KeyPrefix + clientName);
        
        if (!db.KeyExists(key))
        {
            db.StringSet(key,
                _settings.Value.MaxRequestsPerMinute,
                KeyTtl,
                When.NotExists);
        }

        return db.StringDecrement(key);
    }
}
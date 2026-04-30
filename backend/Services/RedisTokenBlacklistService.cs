using StackExchange.Redis;
using backend.Services.Interfaces;

namespace backend.Services;

public class RedisTokenBlacklistService : IRedisTokenBlacklistService
{
    private readonly IDatabase _database;
    private readonly string _keyPrefix;

    public RedisTokenBlacklistService(IConnectionMultiplexer redis, IConfiguration configuration)
    {
        _database = redis.GetDatabase();
        _keyPrefix = configuration["Redis:AccessTokenBlacklistPrefix"] ?? "auth:blacklist:access:";
    }

    public async Task BlacklistAccessTokenAsync(string jti, DateTime expiresAtUtc)
    {
        var ttl = expiresAtUtc - DateTime.UtcNow;
        if (ttl <= TimeSpan.Zero)
        {
            return;
        }

        await _database.StringSetAsync(BuildKey(jti), "1", ttl);
    }

    public async Task<bool> IsBlacklistedAsync(string jti)
    {
        return await _database.KeyExistsAsync(BuildKey(jti));
    }

    private string BuildKey(string jti)
    {
        return $"{_keyPrefix}{jti}";
    }
}

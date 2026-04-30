using backend.Services.Interfaces;
namespace backend.Services;
public class FakeRedisTokenBlacklistService : IRedisTokenBlacklistService
{
    public Task<bool> IsBlacklistedAsync(string jti)
    {
        return Task.FromResult(false);
    }

    public Task BlacklistAccessTokenAsync(string jti, DateTime expiresAt)
    {
        return Task.CompletedTask;
    }
}
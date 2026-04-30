namespace backend.Services.Interfaces;
public interface IRedisTokenBlacklistService
{
    Task<bool> IsBlacklistedAsync(string jti);
    Task BlacklistAccessTokenAsync(string jti, DateTime expiresAt);
}
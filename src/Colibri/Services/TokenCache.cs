using Microsoft.Extensions.Caching.Memory;
using Microsoft.IdentityModel.JsonWebTokens;

namespace Colibri.Services;

public sealed class TokenCache
{
    private readonly MemoryCache _cache = new(new MemoryCacheOptions
    {
        ExpirationScanFrequency = TimeSpan.FromMinutes(2),
        CompactionPercentage = 0.2,
        SizeLimit = 100_000
    });

    public bool TryGetValue(string token, out JsonWebToken? cachedSecurityToken)
    {
        return _cache.TryGetValue(token, out cachedSecurityToken);
    }

    public void Set(string token, JsonWebToken? securityToken, TimeSpan ttl)
    {
        _cache.Set(token, securityToken, new MemoryCacheEntryOptions
        {
            Size = 1,
            AbsoluteExpirationRelativeToNow = ttl
        });
    }
}
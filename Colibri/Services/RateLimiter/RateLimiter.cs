using Colibri.Services.RateLimiter.Interfaces;

namespace Colibri.Services.RateLimiter;

public sealed class RateLimiter : IRateLimiter
{
    public bool Allow(int clusterId, int endpointId)
    {
        Console.WriteLine("Rate Limiter Executed");
        
        return true;
    }
}
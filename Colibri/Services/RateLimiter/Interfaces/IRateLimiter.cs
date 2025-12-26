namespace Colibri.Services.RateLimiter.Interfaces;

public interface IRateLimiter
{
    bool Allow(int clusterId, int endpointId);
}
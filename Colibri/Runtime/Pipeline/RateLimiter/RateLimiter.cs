namespace Colibri.Runtime.Pipeline.RateLimiter;

public sealed class RateLimiter
{
    public bool Allow(int clusterId, int endpointId)
    {
        return true; // Заглушка
    }
}
namespace Colibri.Runtime.Pipeline.LoadBalancer;

public sealed class RoundRobinBalancer(int hostsCount) : ILoadBalancer
{
    private int _counter = 0;

    public int SelectHost()
    {
        return Interlocked.Increment(ref _counter) % hostsCount;
    }
}
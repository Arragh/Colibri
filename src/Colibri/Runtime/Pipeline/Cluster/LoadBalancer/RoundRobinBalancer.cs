using System.Runtime.CompilerServices;

namespace Colibri.Runtime.Pipeline.Cluster.LoadBalancer;

public sealed class RoundRobinBalancer(int hostsCount) : ILoadBalancer
{
    private int _counter = 0;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int SelectHost()
    {
        return Interlocked.Increment(ref _counter) % hostsCount;
    }
}
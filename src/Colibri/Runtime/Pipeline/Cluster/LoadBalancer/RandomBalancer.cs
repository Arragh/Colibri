using System.Runtime.CompilerServices;

namespace Colibri.Runtime.Pipeline.Cluster.LoadBalancer;

public sealed class RandomBalancer(int hostsCount) : ILoadBalancer
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int SelectHost()
    {
        return Random.Shared.Next(hostsCount);
    }
}
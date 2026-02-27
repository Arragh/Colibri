namespace Colibri.Runtime.Pipeline.Cluster.LoadBalancer;

public sealed class RandomBalancer(int hostsCount) : ILoadBalancer
{
    public int SelectHost()
    {
        return Random.Shared.Next(hostsCount);
    }
}
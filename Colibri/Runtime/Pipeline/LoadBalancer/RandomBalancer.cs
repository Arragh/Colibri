namespace Colibri.Runtime.Pipeline.LoadBalancer;

public sealed class RandomBalancer(int hostsCount, HostState[] hostStates) : ILoadBalancer
{
    public int SelectHost()
    {
        return Random.Shared.Next(hostsCount);
    }
}
namespace Colibri.Runtime.Pipeline.LoadBalancer;

public sealed class RandomBalancer(HostState[] hostStates) : ILoadBalancer
{
    private int _hostsCount = hostStates.Length;
    
    public int SelectHost()
    {
        return Random.Shared.Next(_hostsCount);
    }
}
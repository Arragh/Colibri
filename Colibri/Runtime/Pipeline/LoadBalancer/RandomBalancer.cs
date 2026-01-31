namespace Colibri.Runtime.Pipeline.LoadBalancer;

public sealed class RandomBalancer(HostState[] hostStates) : ILoadBalancer
{
    private int _hostsCount = hostStates.Length;
    
    public int SelectHost()
    {
        // TODO: В идеале тут надо разделить Degraded и Healthy-хосты и работать только по Healthy, но для MVP и так сойдет.
        for (int i = 0; i < _hostsCount; i++)
        {
            var hostIdx = Random.Shared.Next(_hostsCount);

            if (Volatile.Read(ref hostStates[hostIdx].State) != 1)
            {
                return hostIdx;
            }
        }
        
        return Random.Shared.Next(_hostsCount);
    }
}
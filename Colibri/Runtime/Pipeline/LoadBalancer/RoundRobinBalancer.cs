namespace Colibri.Runtime.Pipeline.LoadBalancer;

public sealed class RoundRobinBalancer(HostState[] hostStates) : ILoadBalancer
{
    private int _counter = 0;
    private int _hostsCount = hostStates.Length;

    public int SelectHost()
    {
        for (int i = 0; i < _hostsCount; i++)
        {
            var hostIdx = Interlocked.Increment(ref _counter) % _hostsCount;

            if (Volatile.Read(ref hostStates[hostIdx].State) != 1)
            {
                return hostIdx;
            }
        }
        
        return Interlocked.Increment(ref _counter) % _hostsCount;
    }
}
using Colibri.Services.LoadBalancer.Interfaces;

namespace Colibri.Services.LoadBalancer;

public sealed class LoadBalancer : ILoadBalancer
{
    private int _counter;

    public int SelectHost(int clusterId)
    {
        Console.WriteLine("Load Balancer Executed");
        
        return Interlocked.Increment(ref _counter) % 3;
    }
}
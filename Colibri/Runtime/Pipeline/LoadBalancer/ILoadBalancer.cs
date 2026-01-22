namespace Colibri.Runtime.Pipeline.LoadBalancer;

public interface ILoadBalancer
{
    int SelectHost();
}
namespace Colibri.Runtime.Pipeline.Cluster.LoadBalancer;

public interface ILoadBalancer
{
    int SelectHost();
}
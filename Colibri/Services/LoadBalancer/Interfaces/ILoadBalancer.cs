namespace Colibri.Services.LoadBalancer.Interfaces;

public interface ILoadBalancer
{
    int SelectHost(int clusterId);
}
using Colibri.Services.RoutingState.Models;

namespace Colibri.Services.LoadBalancer.Interfaces;

public interface ILoadBalancer
{
    int Select(ClusterConfig clusterConfig);
}
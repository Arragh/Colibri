using Colibri.Services.RoutingState.Models;

namespace Colibri.Services.CircuitBreaker.Interfaces;

public interface ICircuitBreaker
{
    bool CanInvoke(ClusterConfig cluster, int endpointIndex);
    void ReportResult(ClusterConfig cluster, int endpointIndex, bool success);
}
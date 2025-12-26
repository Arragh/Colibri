using Colibri.Services.RoutingState.Models;

namespace Colibri.Services.RoutingEngine.Models;

public class RoutingMatchResult
{
    public required ClusterConfig Cluster { get; init; }
    public required EndpointConfig Endpoint { get; init; }
}
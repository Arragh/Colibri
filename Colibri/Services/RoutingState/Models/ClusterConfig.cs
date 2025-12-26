using Colibri.Services.UpstreamRequestFactory.Enums;

namespace Colibri.Services.RoutingState.Models;

public sealed class ClusterConfig
{
    public required string Prefix { get; set; }
    public required Protocol Protocol { get; set; }
    public required Uri[] Hosts { get; init; }
    public required EndpointConfig[] Endpoints { get; init; }
}
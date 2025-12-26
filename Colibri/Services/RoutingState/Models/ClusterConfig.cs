using Colibri.Services.UpstreamPipeline.Enums;

namespace Colibri.Services.RoutingState.Models;

public sealed class ClusterConfig
{
    public required string Prefix { get; set; }
    public required Protocol Protocol { get; set; }
    public required string[] Hosts { get; init; }
    public required EndpointConfig[] Endpoints { get; init; }
}
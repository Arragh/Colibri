namespace Colibri.Services.RoutingState.Models;

public sealed class EndpointConfig
{
    public required string Method { get; set; }
    public required string UpstreamPattern { get; init; }
    public required string DownstreamPattern { get; init; }
    public required RouteSegment[] CachedUpstream { get; set; }
}
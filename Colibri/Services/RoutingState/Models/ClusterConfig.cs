namespace Colibri.Services.RoutingState.Models;

public sealed class ClusterConfig
{
    public required string Prefix { get; set; }
    public required string[] BaseUrls { get; init; }
    public required EndpointConfig[] Endpoints { get; init; }
}
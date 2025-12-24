namespace Colibri.Services.RoutingState.Models;

public sealed class Cluster
{
    public required string Prefix { get; set; }
    public required string[] BaseUrls { get; init; }
    public required ClusterEndpoint[] Endpoints { get; init; }
}
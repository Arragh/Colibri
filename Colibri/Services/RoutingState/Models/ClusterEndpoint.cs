namespace Colibri.Services.RoutingState.Models;

public sealed class ClusterEndpoint
{
    public required string Method { get; set; }
    public required string Downstream { get; init; }
    public required string Upstream { get; init; }
}
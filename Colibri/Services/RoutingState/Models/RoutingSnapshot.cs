namespace Colibri.Services.RoutingState.Models;

public class RoutingSnapshot
{
    public required ClusterConfig[] Clusters { get; init; }
}
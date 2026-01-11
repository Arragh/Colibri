using Colibri.Runtime.Snapshots.Cluster;
using Colibri.Runtime.Snapshots.Routing;

namespace Colibri.Runtime.Snapshots;

public sealed class GlobalSnapshot
{
    public required ClusterSnapshot ClusterSnapshot { get; init; }
    public required RoutingSnapshot RoutingSnapshot { get; init; }
}
using Colibri.Configuration;
using Colibri.Runtime.Snapshots.Cluster;
using Colibri.Runtime.Snapshots.Routing;

namespace Colibri.Runtime.Snapshots;

public sealed class GlobalSnapshotBuilder
{
    private readonly ClusterSnapshotBuilder _clusterSnapshotBuilder = new();
    private readonly RoutingSnapshotBuilder _routingSnapshotBuilder = new();
    
    public GlobalSnapshot Build(ColibriSettings settings)
    {
        return new GlobalSnapshot
        {
            ClusterSnapshot = _clusterSnapshotBuilder.Build(settings),
            RoutingSnapshot = _routingSnapshotBuilder.Build(settings)
        };
    }
}
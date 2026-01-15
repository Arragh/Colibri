using Colibri.Configuration;
using Colibri.Runtime.Snapshots.Cluster;
using Colibri.Runtime.Snapshots.Routing;

namespace Colibri.Runtime.Snapshots;

public sealed class GlobalSnapshotBuilder
{
    public GlobalSnapshot Build(ColibriSettings settings)
    {
        return new GlobalSnapshot
        {
            ClusterSnapshot = ClusterSnapshotBuilder.Build(settings),
            RoutingSnapshot = RoutingSnapshotBuilder.Build(settings)
        };
    }
}
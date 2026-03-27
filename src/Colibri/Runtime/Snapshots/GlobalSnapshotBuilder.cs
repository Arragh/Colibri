using Colibri.Configuration;
using Colibri.Runtime.Snapshots.Cluster;
using Colibri.Runtime.Snapshots.Routing;
using Microsoft.Extensions.Caching.Memory;

namespace Colibri.Runtime.Snapshots;

public sealed class GlobalSnapshotBuilder(IMemoryCache cache)
{
    private readonly ClusterSnapshotBuilder _clusterSnapshotBuilder = new(cache);
    private readonly RoutingSnapshotBuilder _routingSnapshotBuilder = new();
    
    public GlobalSnapshot Build(ColibriSettings settings)
    {
        settings.Clusters = settings.Clusters
            .Where(c => c.Enabled)
            .ToArray();
        
        return new GlobalSnapshot
        {
            ClusterSnapshot = _clusterSnapshotBuilder.Build(settings.JwtSchemes, settings.Clusters),
            RoutingSnapshot = _routingSnapshotBuilder.Build(settings)
        };
    }
}
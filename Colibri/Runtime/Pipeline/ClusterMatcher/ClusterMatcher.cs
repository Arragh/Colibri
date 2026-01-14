using Colibri.Runtime.Snapshots.Routing;

namespace Colibri.Runtime.Pipeline.ClusterMatcher;

public sealed class ClusterMatcher
{
    public bool TryMatch(
        ReadOnlySpan<char> path,
        RoutingSnapshot routingSnapshot,
        out int clusterId)
    {
        clusterId = 0;
        
        var clusters = routingSnapshot.Clusters;
        var clusterNames = routingSnapshot.ClusterNames;
        
        for (int i = 0; i < clusters.Length; i++)
        {
            ref readonly var clusterSegment = ref clusters[i];
            
            var clusterName = clusterNames
                .Slice(clusterSegment.PathStartIndex, clusterSegment.PathLength);
            
            if (path.StartsWith(clusterName)
                && path.Length > clusterName.Length
                && path[clusterName.Length] == '/')
            {
                clusterId = i;
                return true;
            }
        }

        return false;
    }
}
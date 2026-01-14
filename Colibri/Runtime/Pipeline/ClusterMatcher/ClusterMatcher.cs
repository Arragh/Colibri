using Colibri.Runtime.Snapshots.Routing;

namespace Colibri.Runtime.Pipeline.ClusterMatcher;

public sealed class ClusterMatcher
{
    public bool TryMatch(
        ReadOnlySpan<char> path,
        RoutingSnapshot routingSnapshot,
        out int clusterId,
        out ushort firstChildIndex,
        out ushort childrenCount)
    {
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
                firstChildIndex = clusterSegment.FirstChildIndex;
                childrenCount = clusterSegment.ChildrenCount;
                return true;
            }
        }

        clusterId = 0;
        firstChildIndex = 0;
        childrenCount = 0;
        return false;
    }
}
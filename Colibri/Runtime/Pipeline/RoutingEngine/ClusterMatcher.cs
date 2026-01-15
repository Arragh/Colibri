using Colibri.Runtime.Snapshots.Routing;

namespace Colibri.Runtime.Pipeline.RoutingEngine;

public sealed class ClusterMatcher
{
    public bool TryMatch(
        ReadOnlySpan<char> path,
        RoutingSnapshot routingSnapshot,
        out int clusterId,
        out byte clusterLength,
        out ushort firstUpstreamIndex,
        out ushort upstreamsCount)
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
                clusterLength = (byte)clusterName.Length;
                firstUpstreamIndex = clusterSegment.FirstChildIndex;
                upstreamsCount = clusterSegment.ChildrenCount;
                return true;
            }
        }

        clusterId = 0;
        clusterLength = 0;
        firstUpstreamIndex = 0;
        upstreamsCount = 0;
        return false;
    }
}
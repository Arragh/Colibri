using Colibri.Configuration;

namespace Colibri.Runtime.Snapshots.Routing;

public sealed class RoutingSnapshotBuilder
{
    private readonly RoutesBuilder _routesBuilder = new();
    private readonly TrieBuilder _trieBuilder = new();
    private readonly SnapshotDataBuilder _snapshotDataBuilder = new();
    
    public RoutingSnapshot Build(ColibriSettings settings)
    {
        var tempRoutes = _routesBuilder.Build(settings.Routing.Clusters, settings.Routing.Routes);
        var trie = _trieBuilder.Build(tempRoutes);
        var rootSegmentsCount = trie.Children.Count;
        var snapshotData = _snapshotDataBuilder.Build(trie);
        
        var upstreamSegments = new UpstreamSegment[snapshotData.TempUpstreamSegments.Length];
        var downstreams = new Downstream[snapshotData.TempDownstreams.Length];
        var downstreamSegments = new DownstreamSegment[snapshotData.TempDownstreamSegments.Length];

        for (int i = 0; i < snapshotData.TempUpstreamSegments.Length; i++)
        {
            upstreamSegments[i] = new UpstreamSegment(
                snapshotData.TempUpstreamSegments[i].PathStartIndex,
                snapshotData.TempUpstreamSegments[i].PathLength,
                snapshotData.TempUpstreamSegments[i].FirstChildIndex,
                snapshotData.TempUpstreamSegments[i].ChildrenCount,
                snapshotData.TempUpstreamSegments[i].IsParameter,
                snapshotData.TempUpstreamSegments[i].ParamIndex,
                snapshotData.TempUpstreamSegments[i].DownstreamStartIndex,
                snapshotData.TempUpstreamSegments[i].DownstreamsCount,
                snapshotData.TempUpstreamSegments[i].HasDownstream);
        }

        for (int i = 0; i < snapshotData.TempDownstreams.Length; i++)
        {
            downstreams[i] = new Downstream(
                snapshotData.TempDownstreams[i].ClusterId,
                snapshotData.TempDownstreams[i].FirstChildIndex,
                snapshotData.TempDownstreams[i].ChildrenCount,
                snapshotData.TempDownstreams[i].MethodMask);
        }

        for (int i = 0; i < snapshotData.TempDownstreamSegments.Length; i++)
        {
            downstreamSegments[i] = new DownstreamSegment(
                snapshotData.TempDownstreamSegments[i].PathStartIndex,
                snapshotData.TempDownstreamSegments[i].PathLength,
                snapshotData.TempDownstreamSegments[i].IsParameter,
                snapshotData.TempDownstreamSegments[i].ParamIndex);
        }
        
        return new RoutingSnapshot(
            rootSegmentsCount,
            upstreamSegments,
            snapshotData.TempUpstreamSegmentPaths,
            downstreams,
            downstreamSegments,
            snapshotData.TempDownstreamSegmentPaths);
    }
}
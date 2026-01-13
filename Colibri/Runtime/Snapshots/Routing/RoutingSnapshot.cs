namespace Colibri.Runtime.Snapshots.Routing;

public sealed class RoutingSnapshot(
    ClusterSegment[] clusterSegments,
    char[] clusterNames,
    UpstreamSegment[] upstreamSegments,
    char[] upstreamSegmentPaths,
    Downstream[] downstreams,
    DownstreamSegment[] downstreamSegments,
    char[] downstreamSegmentPaths)
{
    public ReadOnlySpan<ClusterSegment> Clusters => clusterSegments;
    public ReadOnlySpan<char> ClusterNames => clusterNames;
    
    public ReadOnlySpan<UpstreamSegment> UpstreamSegments => upstreamSegments;
    public ReadOnlySpan<char> UpstreamSegmentPaths => upstreamSegmentPaths;
    
    public ReadOnlySpan<Downstream> Downstreams => downstreams;
    public ReadOnlySpan<DownstreamSegment> DownstreamSegments => downstreamSegments;
    public ReadOnlySpan<char> DownstreamSegmentPaths => downstreamSegmentPaths;
}
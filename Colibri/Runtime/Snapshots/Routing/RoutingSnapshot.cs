namespace Colibri.Runtime.Snapshots.Routing;

public sealed class RoutingSnapshot(
    int rootSegmentsCount,
    UpstreamSegment[] upstreamSegments,
    char[] upstreamSegmentPaths,
    Downstream[] downstreams,
    DownstreamSegment[] downstreamSegments,
    char[] downstreamSegmentPaths)
{
    public int RootSegmentsCount = rootSegmentsCount;
    
    public ReadOnlySpan<UpstreamSegment> UpstreamSegments => upstreamSegments;
    public ReadOnlySpan<char> UpstreamSegmentPaths => upstreamSegmentPaths;
    
    public ReadOnlySpan<Downstream> Downstreams => downstreams;
    public ReadOnlySpan<DownstreamSegment> DownstreamSegments => downstreamSegments;
    public ReadOnlySpan<char> DownstreamSegmentPaths => downstreamSegmentPaths;
}
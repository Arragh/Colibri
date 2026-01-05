namespace Colibri.Services.SnapshotProvider.Models.RoutingSnapshot;

public sealed class RoutingSnapshot(
    int rootSegmentsCount,
    Segment[] segments,
    char[] upstreamPathChars,
    Downstream[] downstreams,
    char[] downstreamPathChars,
    Uri[] hosts)
{
    public int RootSegmentsCount => rootSegmentsCount;
    public ReadOnlySpan<Segment> Segments => segments;
    public ReadOnlySpan<char> SegmentNames => upstreamPathChars;
    public ReadOnlySpan<Downstream> Downstreams => downstreams;
    public ReadOnlySpan<char> DownstreamRoutes => downstreamPathChars;
    public ReadOnlySpan<Uri> Hosts => hosts;
}
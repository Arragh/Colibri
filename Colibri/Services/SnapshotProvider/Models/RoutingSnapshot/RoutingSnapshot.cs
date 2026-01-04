namespace Colibri.Services.SnapshotProvider.Models.RoutingSnapshot;

public sealed class RoutingSnapshot(
    Segment[] segments,
    char[] upstreamPathChars,
    Downstream[] downstreams,
    char[] downstreamPathChars)
{
    public ReadOnlySpan<Segment> Segments => segments;
    public ReadOnlySpan<char> SegmentsNames => upstreamPathChars;
    public ReadOnlySpan<Downstream> Downstreams => downstreams;
    public ReadOnlySpan<char> DownstreamRoutes => downstreamPathChars;
}
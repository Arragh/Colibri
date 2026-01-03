using System.Collections.Immutable;

namespace Colibri.Services.SnapshotProvider.Models.RoutingSnapshot;

public readonly struct RoutingSnapshot(Segment[] segments, char[] segmentsNames)
{
    public readonly ImmutableArray<Segment> Segments = segments.ToImmutableArray();
    public readonly ImmutableArray<char> SegmentsNames = segmentsNames.ToImmutableArray();
    public readonly ImmutableArray<char> DownstreamRoutes;
}
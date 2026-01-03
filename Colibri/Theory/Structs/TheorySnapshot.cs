using System.Collections.Immutable;

namespace Colibri.Theory.Structs;

public readonly struct TheorySnapshot(Segment[] segments, char[] segmentsNames)
{
    public readonly ImmutableArray<Segment> Segments = segments.ToImmutableArray();
    public readonly ImmutableArray<char> SegmentsNames = segmentsNames.ToImmutableArray();
}
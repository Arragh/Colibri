namespace Colibri.Services.SnapshotProvider.Models.RoutingSnapshot;

public readonly struct Segment(
    int pathStartIndex,
    short pathLength,
    int firstChildIndex,
    short childrenCount,
    short downstreamStartIndex,
    byte downstreamCount,
    byte methodMask)
{
    public readonly int PathStartIndex = pathStartIndex;
    public readonly int FirstChildIndex = firstChildIndex;
    public readonly short PathLength = pathLength;
    public readonly short ChildrenCount = childrenCount;
    public readonly short DownstreamStartIndex = downstreamStartIndex;
    public readonly byte DownstreamCount = downstreamCount;
    public readonly byte MethodMask = methodMask;
}
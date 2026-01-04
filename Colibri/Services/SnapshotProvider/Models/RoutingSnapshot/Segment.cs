namespace Colibri.Services.SnapshotProvider.Models.RoutingSnapshot;

public readonly struct Segment(
    int pathStartIndex,
    short pathLength,
    int firstChildIndex,
    short childrenCount,
    byte methodMask)
{
    public readonly int PathStartIndex = pathStartIndex;
    public readonly int FirstChildIndex = firstChildIndex;
    public readonly short PathLength = pathLength;
    public readonly short ChildrenCount = childrenCount;
    public readonly short DownstreamStartIndex;
    public readonly byte DownstreamCount;
    public readonly byte MethodMask = methodMask;
}
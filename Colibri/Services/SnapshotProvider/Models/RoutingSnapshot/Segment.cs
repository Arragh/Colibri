namespace Colibri.Services.SnapshotProvider.Models.RoutingSnapshot;

public readonly struct Segment(
    int pathStartIndex,
    int pathLength,
    int firstChildIndex,
    int childrenCount)
{
    public readonly int PathStartIndex = pathStartIndex;
    public readonly int PathLength = pathLength;

    public readonly int FirstChildIndex = firstChildIndex;
    public readonly int ChildrenCount = childrenCount;
}
namespace Colibri.Runtime.Snapshots.Routing;

public readonly struct ClusterSegment(
    int pathStartIndex,
    int pathLength,
    int firstChildIndex,
    int childrenCount)
{
    public readonly int PathStartIndex = pathStartIndex;
    public readonly int FirstChildIndex = firstChildIndex;
    public readonly int ChildrenCount = childrenCount;
    public readonly int PathLength = pathLength;
}
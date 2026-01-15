namespace Colibri.Runtime.Snapshots.Routing;

public readonly struct ClusterSegment(
    ushort pathStartIndex,
    byte pathLength,
    ushort firstChildIndex,
    ushort childrenCount)
{
    public readonly ushort FirstChildIndex = firstChildIndex;
    public readonly ushort ChildrenCount = childrenCount;
    public readonly ushort PathStartIndex = pathStartIndex;
    public readonly byte PathLength = pathLength;
}
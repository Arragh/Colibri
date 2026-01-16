namespace Colibri.Runtime.Snapshots.Routing;

public readonly struct UpstreamSegment(
    int pathStartIndex,
    byte pathLength,
    ushort firstChildIndex,
    ushort childrenCount,
    bool isParameter,
    byte paramIndex,
    ushort downstreamIndex,
    bool hasDownstream)
{
    public readonly int PathStartIndex = pathStartIndex;
    public readonly ushort FirstChildIndex = firstChildIndex;
    public readonly ushort ChildrenCount = childrenCount;
    public readonly ushort DownstreamIndex = downstreamIndex;
    public readonly bool HasDownstream = hasDownstream;
    public readonly byte PathLength = pathLength;
    public readonly bool IsParameter = isParameter;
    public readonly byte ParamIndex = paramIndex;
}
namespace Colibri.Runtime.Snapshots.Routing;

public readonly struct UpstreamSegment(
    int pathStartIndex,
    byte pathLength,
    ushort firstChildIndex,
    ushort childrenCount,
    bool isParameter,
    byte paramIndex,
    ushort downstreamStartIndex,
    byte downstreamsCount,
    bool hasDownstream)
{
    public readonly int PathStartIndex = pathStartIndex;
    public readonly ushort FirstChildIndex = firstChildIndex;
    public readonly ushort ChildrenCount = childrenCount;
    public readonly ushort DownstreamStartIndex = downstreamStartIndex;
    public readonly byte DownstreamsCount = downstreamsCount;
    public readonly bool HasDownstream = hasDownstream;
    public readonly bool IsParameter = isParameter;
    public readonly byte ParamIndex = paramIndex;
    public readonly byte PathLength = pathLength;
}
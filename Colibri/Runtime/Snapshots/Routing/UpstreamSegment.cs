namespace Colibri.Runtime.Snapshots.Routing;

public readonly struct UpstreamSegment(
    int pathStartIndex,
    byte pathLength,
    int firstChildIndex,
    ushort childrenCount,
    bool isParameter,
    byte paramIndex,
    ushort downstreamStartIndex,
    byte downstreamsCount)
{
    public readonly int PathStartIndex = pathStartIndex;
    public readonly int FirstChildIndex = firstChildIndex;
    public readonly ushort ChildrenCount = childrenCount;
    public readonly ushort DownstreamStartIndex = downstreamStartIndex;
    public readonly byte PathLength = pathLength;
    public readonly bool IsParameter = isParameter;
    public readonly byte ParamIndex = paramIndex;
    public readonly byte DownstreamsCount = downstreamsCount;
}
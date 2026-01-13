namespace Colibri.Runtime.Snapshots.Routing;

public readonly struct UpstreamSegment(
    int pathStartIndex,
    int pathLength,
    int firstChildIndex,
    int childrenCount,
    bool isParameter,
    int paramIndex,
    int downstreamStartIndex,
    int downstreamsCount)
{
    public readonly int PathStartIndex = pathStartIndex;
    public readonly int PathLength = pathLength;
    public readonly int FirstChildIndex = firstChildIndex;
    public readonly int ChildrenCount = childrenCount;
    public readonly bool IsParameter = isParameter;
    public readonly int ParamIndex = paramIndex;
    public readonly int DownstreamStartIndex = downstreamStartIndex;
    public readonly int DownstreamsCount = downstreamsCount;
}
namespace Colibri.Runtime.Snapshots.Routing;

public readonly struct DownstreamSegment(
    int pathStartIndex,
    int pathLength,
    bool isParameter,
    int paramIndex)
{
    public readonly int PathStartIndex = pathStartIndex;
    public readonly int PathLength = pathLength;
    public readonly bool IsParameter = isParameter;
    public readonly int ParamIndex = paramIndex;
}
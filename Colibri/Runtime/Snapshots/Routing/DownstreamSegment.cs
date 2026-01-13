namespace Colibri.Runtime.Snapshots.Routing;

public readonly struct DownstreamSegment(
    int pathStartIndex,
    byte pathLength,
    bool isParameter,
    byte paramIndex)
{
    public readonly int PathStartIndex = pathStartIndex;
    public readonly byte PathLength = pathLength;
    public readonly bool IsParameter = isParameter;
    public readonly byte ParamIndex = paramIndex;
}
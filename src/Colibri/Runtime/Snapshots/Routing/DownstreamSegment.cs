namespace Colibri.Runtime.Snapshots.Routing;

public readonly struct DownstreamSegment(TempDownstreamSegment tempSegment)
{
    public readonly int PathStartIndex = tempSegment.PathStartIndex;
    public readonly byte PathLength = tempSegment.PathLength;
    public readonly bool IsParameter = tempSegment.IsParameter;
    public readonly byte ParamIndex = tempSegment.ParamIndex;
}
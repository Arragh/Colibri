namespace Colibri.Runtime.Snapshots.Routing;

public readonly struct UpstreamSegment(TempUpstreamSegment tempSegment)
{
    public readonly int PathStartIndex = tempSegment.PathStartIndex;
    public readonly ushort FirstChildIndex = tempSegment.FirstChildIndex;
    public readonly ushort ChildrenCount = tempSegment.ChildrenCount;
    public readonly ushort DownstreamStartIndex = tempSegment.DownstreamStartIndex;
    public readonly byte DownstreamsCount = tempSegment.DownstreamsCount;
    public readonly bool HasDownstream = tempSegment.HasDownstream;
    public readonly bool IsParameter = tempSegment.IsParameter;
    public readonly byte ParamIndex = tempSegment.ParamIndex;
    public readonly byte PathLength = tempSegment.PathLength;
}
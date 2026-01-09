namespace Colibri.Snapshots.RoutingSnapshot.Models;

public class TempDownstreamChunk
{
    public string Name { get; set; }
    public int PathStartIndex { get; set; }
    public int PathLength { get; set; }
    public bool IsParameter { get; set; }
    public short ParamIndex { get; set; }
}
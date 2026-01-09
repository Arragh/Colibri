namespace Colibri.Snapshots.RoutingSnapshot.Models;

public class TempDownstream
{
    public int PathStartIndex { get; set; }
    public short PathLength { get; set; }
    public byte MethodMask { get; set; }
    
    public short HostStartIndex { get; set; }
    public byte HostsCount { get; set; }
    
    public int ChunkStartIndex { get; set; }
    public int ChunksCount { get; set; }
}
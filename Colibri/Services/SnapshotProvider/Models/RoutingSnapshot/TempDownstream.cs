namespace Colibri.Services.SnapshotProvider.Models.RoutingSnapshot;

public class TempDownstream
{
    public int PathStartIndex { get; set; }
    public short PathLength { get; set; }
    public byte MethodMask { get; set; }
    
    public short HostStartIndex { get; set; }
    public byte HostsCount { get; set; }
}
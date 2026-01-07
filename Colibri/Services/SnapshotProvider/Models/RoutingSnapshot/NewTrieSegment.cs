namespace Colibri.Services.SnapshotProvider.Models.RoutingSnapshot;

public class NewTrieSegment
{
    public string SegmentName { get; set; }
    public bool IsParameter { get; set; }
    public int HostStartIndex { get; set; }
    public int HostsCount { get; set; }
    
    public List<NewTrieSegment> ChildrenSegments { get; set; } = new();
    public Dictionary<string, string> Methods { get; set; } = new();
}
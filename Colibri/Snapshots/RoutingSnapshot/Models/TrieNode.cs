namespace Colibri.Snapshots.RoutingSnapshot.Models;

public class TrieNode
{
    public string? SegmentName { get; set; }
    public bool IsParameter { get; set; }
    public int HostStartIndex { get; set; }
    public int HostsCount { get; set; }
    
    public List<TrieNode> ChildrenSegments { get; set; } = new();
    public Dictionary<string, string> Methods { get; set; } = new();
}
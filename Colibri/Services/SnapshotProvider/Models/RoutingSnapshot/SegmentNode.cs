namespace Colibri.Services.SnapshotProvider.Models.RoutingSnapshot;

public sealed class SegmentNode
{
    public string SegmentName { get; set; }
    public Dictionary<string, SegmentNode> IncludedSegments { get; set; } = new();
    public Dictionary<string, string> Methods { get; set; } = new();
}
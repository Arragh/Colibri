namespace Colibri.Theory.Models;

public class SegmentNode
{
    public string SegmentName { get; set; }
    public Dictionary<string, SegmentNode> IncludedSegments { get; set; } = new();
    public Dictionary<string, string> Methods { get; set; } = new();
}
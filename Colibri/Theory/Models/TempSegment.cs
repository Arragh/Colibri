namespace Colibri.Theory.Models;

public class TempSegment
{
    public string SegmentName { get; set; }
    public Dictionary<string, TempSegment> IncludedSegments { get; set; } = new();
    public Dictionary<string, string> Methods { get; set; } = new();
}
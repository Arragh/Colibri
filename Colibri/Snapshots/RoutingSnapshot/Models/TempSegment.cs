namespace Colibri.Snapshots.RoutingSnapshot.Models;

public sealed class TempSegment
{
    public int PathStartIndex { get; set; }
    public short PathLength { get; set;}

    public int FirstChildIndex { get; set;}
    public short ChildrenCount { get; set;}
    
    public byte MethodMask { get; set;}
    
    public short DownstreamStartIndex { get; set;}
    public byte DownstreamCount { get; set; } = 0;
}
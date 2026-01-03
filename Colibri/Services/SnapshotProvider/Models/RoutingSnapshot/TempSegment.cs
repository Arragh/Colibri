namespace Colibri.Services.SnapshotProvider.Models.RoutingSnapshot;

public sealed class TempSegment
{
    public int PathStartIndex { get; set; }
    public int PathLength { get; set;}

    public int FirstChildIndex { get; set;}
    public int ChildrenCount { get; set;}
    
    public int MethodMask { get; set;}
}
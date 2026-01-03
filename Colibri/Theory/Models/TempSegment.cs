namespace Colibri.Theory.Models;

public class TempSegment
{
    public int PathStartIndex { get; set; }
    public int PathLength { get; set;}

    public int FirstChildIndex { get; set;}
    public int ChildrenCount { get; set;}
}
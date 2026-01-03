namespace Colibri.Theory.Structs;

public class Segment
{
    public int Id { get; set; }
    
    public int PathStartIndex { get; set; }
    public int PathLength { get; set;}

    public int? FirstChildIndex { get; set;}
    public int ChildrenCount { get; set;}
}
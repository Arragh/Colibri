namespace Colibri.Runtime.Snapshots.Routing;

public readonly struct Downstream(TempDownstream tempDownstream)
{
    public readonly ushort ClusterId = tempDownstream.ClusterId;
    public readonly ushort FirstChildIndex = tempDownstream.FirstChildIndex;
    public readonly byte ChildrenCount = tempDownstream.ChildrenCount;
    public readonly byte MethodMask = tempDownstream.MethodMask;
}
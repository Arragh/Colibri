namespace Colibri.Runtime.Snapshots.Routing;

public readonly struct Downstream(
    ushort clusterId,
    ushort firstChildIndex,
    byte childrenCount,
    byte methodMask)
{
    public readonly ushort ClusterId = clusterId;
    public readonly ushort FirstChildIndex = firstChildIndex;
    public readonly byte ChildrenCount = childrenCount;
    public readonly byte MethodMask = methodMask;
}
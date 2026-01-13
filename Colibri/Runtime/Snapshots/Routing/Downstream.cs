namespace Colibri.Runtime.Snapshots.Routing;

public readonly struct Downstream(
    int firstChildIndex,
    int childrenCount,
    byte methodMask)
{
    public readonly int FirstChildIndex = firstChildIndex;
    public readonly int ChildrenCount = childrenCount;
    public readonly byte MethodMask = methodMask;
}
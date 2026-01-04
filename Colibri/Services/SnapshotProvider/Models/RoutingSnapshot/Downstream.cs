namespace Colibri.Services.SnapshotProvider.Models.RoutingSnapshot;

public readonly struct Downstream(
    int pathStartIndex,
    short pathLength,
    byte methodMask)
{
    public readonly int PathStartIndex = pathStartIndex;
    public readonly short PathLength = pathLength;
    public readonly byte MethodMask = methodMask;
}
namespace Colibri.Services.SnapshotProvider.Models.RoutingSnapshot;

public readonly struct Downstream(
    int pathStartIndex,
    short pathLength,
    byte methodMask,
    short hostStartIndex,
    byte hostsCount)
{
    public readonly int PathStartIndex = pathStartIndex;
    public readonly short PathLength = pathLength;
    public readonly short HostStartIndex = hostStartIndex;
    public readonly byte HostsCount = hostsCount;
    public readonly byte MethodMask = methodMask;
}
namespace Colibri.Runtime.Snapshots.Routing;

public struct Prefix(
    int prefixStartIndex,
    byte prefixLength,
    int downstreamStartIndex,
    short downstreamsCount)
{
    public readonly int PrefixStartIndex = prefixStartIndex;
    public readonly byte PrefixLength = prefixLength;

    public readonly int DownstreamStartIndex = downstreamStartIndex;
    public readonly short DownstreamsCount = downstreamsCount;
}
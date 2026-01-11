namespace Colibri.Runtime.Snapshots.Routing;

public sealed class RoutingSnapshot(
    Prefix[] prefixes,
    char[] prefixesNames)
{
    public ReadOnlySpan<Prefix> Prefixes => prefixes;
    public ReadOnlySpan<char> PrefixesNames => prefixesNames;
}
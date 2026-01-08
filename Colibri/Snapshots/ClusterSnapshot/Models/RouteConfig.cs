using System.Collections.Immutable;

namespace Colibri.Snapshots.ClusterSnapshot.Models;

public sealed class RouteConfig
{
    public required string Method { get; init; }
    public required string UpstreamPattern { get; init; }
    public required string DownstreamPattern { get; init; }
    public required ImmutableArray<RouteSegment> CachedUpstream { get; init; }
}
using System.Collections.Immutable;
using Colibri.Snapshots.Enums;

namespace Colibri.Snapshots.ClusterSnapshot.Models;

public sealed class ClusterConfig
{
    public required Protocol Protocol { get; init; }
    public required ImmutableArray<Uri> Hosts { get; init; }
    public required ImmutableArray<RouteConfig> Routes { get; init; }
}
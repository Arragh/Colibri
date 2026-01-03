using System.Collections.Immutable;
using Colibri.Services.SnapshotProvider.Enums;

namespace Colibri.Services.SnapshotProvider.Models;

public sealed class ClusterConfig
{
    public required Protocol Protocol { get; init; }
    public required ImmutableArray<Uri> Hosts { get; init; }
    public required ImmutableArray<RouteConfig> Routes { get; init; }
}
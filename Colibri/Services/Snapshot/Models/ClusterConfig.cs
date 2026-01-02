using System.Collections.Immutable;
using Colibri.Services.Snapshot.Enums;

namespace Colibri.Services.Snapshot.Models;

public sealed class ClusterConfig
{
    public required Protocol Protocol { get; init; }
    public required ImmutableArray<Uri> Hosts { get; init; }
    public required ImmutableArray<RouteConfig> Routes { get; init; }
}
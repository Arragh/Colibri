using System.Collections.Immutable;

namespace Colibri.Services.Snapshot.Models;

public class ClusterConfigSnapshot
{
    public required ImmutableArray<ClusterConfig> Clusters { get; init; }
}
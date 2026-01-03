using System.Collections.Immutable;

namespace Colibri.Services.SnapshotProvider.Models;

public class ClusterConfigSnapshot
{
    public required ImmutableArray<ClusterConfig> Clusters { get; init; }
}
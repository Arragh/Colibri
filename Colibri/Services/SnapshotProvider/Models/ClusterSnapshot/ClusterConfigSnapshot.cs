using System.Collections.Immutable;

namespace Colibri.Services.SnapshotProvider.Models.ClusterSnapshot;

public sealed class ClusterConfigSnapshot
{
    public required ImmutableArray<ClusterConfig> Clusters { get; init; }
}
using System.Collections.Immutable;
using Colibri.Snapshots.ClusterSnapshot.Models;

namespace Colibri.Snapshots.ClusterSnapshot;

public sealed class ClusterConfigSnapshot
{
    public required ImmutableArray<ClusterConfig> Clusters { get; init; }
}
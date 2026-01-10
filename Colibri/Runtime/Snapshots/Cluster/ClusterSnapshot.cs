using System.Collections.Immutable;

namespace Colibri.Runtime.Snapshots.Cluster;

public sealed class ClusterSnapshot
{
    public required ImmutableArray<ClusterSnp> Clusters { get; init; }
}
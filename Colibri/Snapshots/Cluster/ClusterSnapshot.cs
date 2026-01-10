using System.Collections.Immutable;
using Colibri.Snapshots.Cluster.Models;

namespace Colibri.Snapshots.Cluster;

public sealed class ClusterSnapshot
{
    public required ImmutableArray<ClusterSnp> Clusters { get; init; }
}
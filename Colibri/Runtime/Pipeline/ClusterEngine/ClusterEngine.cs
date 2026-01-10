using Colibri.Services.SnapshotProvider.Interfaces;
using Colibri.Snapshots.Cluster.Models;

namespace Colibri.Runtime.Pipeline.ClusterEngine;

public sealed class ClusterEngine(ISnapshotProvider provider)
{
    public ClusterSnp TryMatch(string prefix)
    {
        return provider.ClusterSnapshot.Clusters.First();
    }
}
using Colibri.Runtime.Snapshots;
using Colibri.Runtime.Snapshots.Cluster;

namespace Colibri.Runtime.Pipeline.ClusterEngine;

public sealed class ClusterEngine(ISnapshotProvider provider)
{
    public ClusterSnp TryMatch(string prefix)
    {
        return provider.ClusterSnapshot.Clusters.First();
    }
}
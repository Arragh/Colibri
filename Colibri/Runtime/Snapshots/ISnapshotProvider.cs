using Colibri.Runtime.Snapshots.Cluster;

namespace Colibri.Runtime.Snapshots;

public interface ISnapshotProvider
{
    ClusterSnapshot ClusterSnapshot { get; }
}
using Colibri.Snapshots.Cluster;

namespace Colibri.Services.SnapshotProvider.Interfaces;

public interface ISnapshotProvider
{
    ClusterSnapshot ClusterSnapshot { get; }
}
using Colibri.Services.Snapshot.Models;

namespace Colibri.Services.Snapshot.Interfaces;

public interface ISnapshotProvider
{
    ClusterConfigSnapshot ClusterSnapshot { get; }
    TransportRuntimeSnapshot TransportSnapshot { get; }
}
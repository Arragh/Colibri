using Colibri.Services.SnapshotProvider.Models.ClusterSnapshot;
using Colibri.Services.SnapshotProvider.Models.TransportSnapshot;

namespace Colibri.Services.SnapshotProvider.Models;

public sealed class GlobalSnapshot
{
    public required ClusterConfigSnapshot ClusterSnapshot { get; init; }
    public required TransportRuntimeSnapshot TransportSnapshot { get; init; }
}
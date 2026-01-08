using Colibri.Snapshots.ClusterSnapshot;
using Colibri.Snapshots.TransportSnapshot;

namespace Colibri.Snapshots;

public sealed class GlobalSnapshot
{
    public required ClusterConfigSnapshot ClusterSnapshot { get; init; }
    public required TransportRuntimeSnapshot TransportSnapshot { get; init; }
}
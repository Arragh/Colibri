namespace Colibri.Services.SnapshotProvider.Models;

public class GlobalSnapshot
{
    public required ClusterConfigSnapshot ClusterSnapshot { get; init; }
    public required TransportRuntimeSnapshot TransportSnapshot { get; init; }
}
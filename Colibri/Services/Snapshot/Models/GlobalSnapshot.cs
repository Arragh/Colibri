namespace Colibri.Services.Snapshot.Models;

public class GlobalSnapshot
{
    public required ClusterConfigSnapshot ClusterSnapshot { get; init; }
    public required TransportRuntimeSnapshot TransportSnapshot { get; init; }
}
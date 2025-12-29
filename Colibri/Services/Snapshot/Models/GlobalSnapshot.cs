namespace Colibri.Services.Snapshot.Models;

public class GlobalSnapshot
{
    public required ClusterConfigSnapshot ClusterConfigSnapshot { get; init; }
    public required TransportRuntimeSnapshot TransportRuntimeSnapshot { get; init; }
}
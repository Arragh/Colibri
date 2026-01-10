using Colibri.Runtime.Snapshots.Cluster;

namespace Colibri.Runtime.Snapshots;

public sealed class GlobalSnapshot
{
    public required ClusterSnapshot ClusterSnapshot { get; set; }
}
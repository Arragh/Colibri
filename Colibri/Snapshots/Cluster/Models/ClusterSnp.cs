using Colibri.Runtime.Pipeline;

namespace Colibri.Snapshots.Cluster.Models;

public sealed class ClusterSnp
{
    public bool Enabled { get; init; }
    public required string ClusterId { get; init; }
    public required string Protocol { get; init; }
    public required string Prefix { get; init; }
    public required Uri[] Hosts { get; init; }
    public required Pipeline Pipeline { get; init; }
}
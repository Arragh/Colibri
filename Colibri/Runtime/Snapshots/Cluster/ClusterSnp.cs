using Colibri.Runtime.Pipeline;

namespace Colibri.Runtime.Snapshots.Cluster;

public sealed class ClusterSnp
{
    public required string ClusterId { get; init; }
    public required string Protocol { get; init; }
    public required string Prefix { get; init; }
    public required int HostsCount { get; init; }
    public required PipelineSrv Pipeline { get; init; }
}
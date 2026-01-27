using System.Collections.Immutable;

namespace Colibri.Runtime.Snapshots.Cluster;

public sealed class ClusterSnapshot : IAsyncDisposable
{
    public required ImmutableArray<ClusterSnp> Clusters { get; init; }

    public async ValueTask DisposeAsync()
    {
        foreach (var cluster in Clusters)
        {
            await cluster.DisposeAsync();
        }
    }
}
using System.Diagnostics;
using Colibri.Runtime.Pipeline;

namespace Colibri.Runtime.Snapshots.Cluster;

public sealed class ClusterSnp : IAsyncDisposable
{
    private int _activitiesCount = 0;
    
    public required string Name { get; init; }
    public required string Protocol { get; init; }
    public required string Prefix { get; init; }
    public required int HostsCount { get; init; }
    public required PipelineSrv Pipeline { get; init; }

    public void Take() => Interlocked.Increment(ref _activitiesCount);
    public void Release() => Interlocked.Decrement(ref _activitiesCount);

    public async ValueTask DisposeAsync()
    {
        var sw = Stopwatch.StartNew();
        
        while (Volatile.Read(ref _activitiesCount) > 0
               && sw.Elapsed < TimeSpan.FromSeconds(30))
        {
            await Task.Delay(100);
        }
        
        Pipeline.Dispose();
    }
}
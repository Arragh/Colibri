using Colibri.Configuration;
using Colibri.Runtime.Snapshots.Cluster;
using Microsoft.Extensions.Options;

namespace Colibri.Runtime.Snapshots;

public sealed class SnapshotProvider : ISnapshotProvider
{
    private GlobalSnapshot _globalSnapshot;
    private SnapshotBuilder _snapshotBuilder = new();
    
    public SnapshotProvider(IOptionsMonitor<ColibriSettings> monitor)
    {
        _globalSnapshot = _snapshotBuilder.Build(monitor.CurrentValue);

        monitor.OnChange(c =>
        {
            Console.WriteLine("SNAPSHOT CHANGED\n\n\n");
            
            var newGlobalSnapshot = _snapshotBuilder.Build(c);
            Volatile.Write(ref _globalSnapshot, newGlobalSnapshot);
            
        });
    }

    public ClusterSnapshot ClusterSnapshot => Volatile.Read(ref _globalSnapshot).ClusterSnapshot;
}
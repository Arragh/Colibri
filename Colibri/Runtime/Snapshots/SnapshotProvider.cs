using Colibri.Configuration;
using Colibri.Runtime.Snapshots.Cluster;
using Microsoft.Extensions.Options;

namespace Colibri.Runtime.Snapshots;

public sealed class SnapshotProvider : ISnapshotProvider
{
    private GlobalSnapshot _globalSnapshot;
    private GlobalSnapshotBuilder _globalSnapshotBuilder = new();
    
    public SnapshotProvider(IOptionsMonitor<ColibriSettings> monitor)
    {
        _globalSnapshot = _globalSnapshotBuilder.Build(monitor.CurrentValue);

        monitor.OnChange(c =>
        {
            Console.WriteLine("SNAPSHOT CHANGED\n\n\n");
            
            var newGlobalSnapshot = _globalSnapshotBuilder.Build(c);
            Volatile.Write(ref _globalSnapshot, newGlobalSnapshot);
            
        });
    }

    public ClusterSnapshot ClusterSnapshot => Volatile.Read(ref _globalSnapshot).ClusterSnapshot;
}
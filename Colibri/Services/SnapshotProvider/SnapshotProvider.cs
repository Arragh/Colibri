using Colibri.Configuration;
using Colibri.Services.SnapshotProvider.Interfaces;
using Colibri.Snapshots;
using Colibri.Snapshots.Cluster;
using Microsoft.Extensions.Options;

namespace Colibri.Services.SnapshotProvider;

public sealed class SnapshotProvider : ISnapshotProvider
{
    private GlobalSnapshot _globalSnapshot;
    private SnapshotBuilder _snapshotBuilder = new();
    
    public SnapshotProvider(IOptionsMonitor<ColibriSettings> monitor)
    {
        _globalSnapshot = _snapshotBuilder.Build(monitor.CurrentValue);
    }

    public ClusterSnapshot ClusterSnapshot => Volatile.Read(ref _globalSnapshot).ClusterSnapshot;
}
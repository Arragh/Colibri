using Colibri.Snapshots;
using Colibri.Snapshots.RoutingSnapshot;

namespace Colibri.Services.SnapshotProvider.Interfaces;

public interface ISnapshotProvider
{
    GlobalSnapshot GlobalSnapshot { get; }
    RoutingSnapshot RoutingSnapshot { get; }
}
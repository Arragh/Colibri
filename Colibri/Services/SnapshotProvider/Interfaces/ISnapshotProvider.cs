using Colibri.Services.SnapshotProvider.Models;
using Colibri.Services.SnapshotProvider.Models.RoutingSnapshot;

namespace Colibri.Services.SnapshotProvider.Interfaces;

public interface ISnapshotProvider
{
    GlobalSnapshot GlobalSnapshot { get; }
    ref readonly RoutingSnapshot RoutingSnapshot { get; }
}
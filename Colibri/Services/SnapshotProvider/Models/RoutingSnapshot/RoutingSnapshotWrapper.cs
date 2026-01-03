namespace Colibri.Services.SnapshotProvider.Models.RoutingSnapshot;

public sealed class RoutingSnapshotWrapper(RoutingSnapshot routingSnapshot)
{
    private RoutingSnapshot _routingSnapshot = routingSnapshot;

    public ref readonly RoutingSnapshot RoutingSnapshot => ref _routingSnapshot;
}
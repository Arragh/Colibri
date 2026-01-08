using Colibri.Services.SnapshotProvider.Models.RoutingSnapshot;

namespace Colibri.Services.RoutingEngine.Interfaces;

public interface IRoutingEngine
{
    bool TryMatch(
        RoutingSnapshot snapshot,
        ReadOnlySpan<char> requestPath,
        byte methodMask,
        out Downstream? result);
}
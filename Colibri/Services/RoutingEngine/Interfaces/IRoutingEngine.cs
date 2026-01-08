using Colibri.Snapshots.RoutingSnapshot;
using Colibri.Snapshots.RoutingSnapshot.Models;

namespace Colibri.Services.RoutingEngine.Interfaces;

public interface IRoutingEngine
{
    bool TryMatch(
        RoutingSnapshot snapshot,
        ReadOnlySpan<char> requestPath,
        byte methodMask,
        out Downstream? result);
}
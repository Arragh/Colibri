using Colibri.Services.RoutingState.Models;

namespace Colibri.Services.RoutingEngine.Interfaces;

public interface IRoutingEngine
{
    bool TryMatchRoute(
        ReadOnlySpan<char> url,
        ReadOnlySpan<char> method,
        RoutingSnapshot snapshot,
        out string downstreamPath);
}
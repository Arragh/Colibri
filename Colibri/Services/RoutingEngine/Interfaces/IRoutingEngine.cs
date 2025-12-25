using Colibri.Services.RoutingState.Models;

namespace Colibri.Services.RoutingEngine.Interfaces;

public interface IRoutingEngine
{
    ClusterConfig? Match(ReadOnlySpan<char> path);
}
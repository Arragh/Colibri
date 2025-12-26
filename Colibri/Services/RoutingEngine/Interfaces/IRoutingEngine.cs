using Colibri.Services.RoutingEngine.Models;

namespace Colibri.Services.RoutingEngine.Interfaces;

public interface IRoutingEngine
{
    RoutingMatchResult? Match(ReadOnlySpan<char> path, HttpMethod method);
}